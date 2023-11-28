using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Iyzipay.Request;
using Iyzipay.Model;
using Iyzipay;
using Inveon.Services.ShoppingCartAPI.RabbitMQSender;
using Inveon.Services.ShoppingCartAPI.Repository;
using Inveon.Models;
using Inveon.Models.DTOs;
using System.Text.Json;

namespace Inveon.Service.ShoppingCartAPI.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/cartc")]
    public class CartAPICheckOutController : ControllerBase
    {

        private readonly ICartRepository _cartRepository;
        private readonly ICouponRepository _couponRepository;
        // private readonly IMessageBus _messageBus;
        protected ResponseDto _response;
        private readonly IRabbitMQCartMessageSender _rabbitMQCartMessageSender;
        // IMessageBus messageBus,
        public CartAPICheckOutController(ICartRepository cartRepository,
            ICouponRepository couponRepository, IRabbitMQCartMessageSender rabbitMQCartMessageSender)
        {
            _cartRepository = cartRepository;
            _couponRepository = couponRepository;
            _rabbitMQCartMessageSender = rabbitMQCartMessageSender;
            //_messageBus = messageBus;
            this._response = new ResponseDto();
        }

        // Payment method
        [HttpPost]
        [Authorize]
        public async Task<object> Checkout([FromBody] CheckoutHeaderDto checkoutHeader)
        {
            try
            {
                CartDto cartDto = _cartRepository.GetCartByUserIdNonAsync(checkoutHeader.UserId);
                if (cartDto == null)
                {
                    return BadRequest();
                }

                if (!string.IsNullOrEmpty(checkoutHeader.CouponCode))
                {
                    CouponDto coupon = await _couponRepository.GetCoupon(checkoutHeader.CouponCode);
                    if (checkoutHeader.DiscountTotal != coupon.DiscountAmount)
                    {
                        _response.IsSuccess = false;
                        _response.ErrorMessages = new List<string>() { "Coupon Price has changed, please confirm" };
                        _response.DisplayMessage = "Coupon Price has changed, please confirm";
                        return _response;
                    }
                }

                checkoutHeader.CartDetails = cartDto.CartDetails;
                //logic to add message to process order.
                // await _messageBus.PublishMessage(checkoutHeader, "checkoutqueue");

                ////rabbitMQ

                Payment payment = OdemeIslemi(checkoutHeader);
                if (payment.Status != "success")
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() {payment.ErrorMessage};
                    return _response;
                }

                _rabbitMQCartMessageSender.SendMessage(checkoutHeader, "checkoutqueue");
                _rabbitMQCartMessageSender.SendMessage(checkoutHeader, "informQueue");
                await _cartRepository.ClearCart(checkoutHeader.UserId);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        public Payment OdemeIslemi(CheckoutHeaderDto checkoutHeaderDto)
        {

            CartDto cartDto = _cartRepository.GetCartByUserIdNonAsync(checkoutHeaderDto.UserId);

            //CartDto cartDto = checkoutHeaderDto.CartDetails;
            Options options = new Options();

            options.ApiKey = "sandbox-Y6LiFuh3u5sPgHzh0i3eoaeDK6a72Idc";
            options.SecretKey = "sandbox-r8qKpYEbYpkRnqQ0I49dxXQxJye4scvK";
            options.BaseUrl = "https://sandbox-api.iyzipay.com";

            // Sepet tutarını kendi databaseimizden gelen bilgiden hesaplıyoruz.
            double totalPrice = 0.0;
            
            foreach (CartDetailsDto productDetails in cartDto.CartDetails)
            {
                var priceEach = productDetails.Product.Price;
                var count = productDetails.Count;
                totalPrice += priceEach * count;
            }
            totalPrice = Math.Round(totalPrice, 2);
            //CouponDto couponDetails = await _couponRepository.GetCoupon(cartDto.CartHeader.CouponCode);
            //double discountAmount = couponDetails.DiscountAmount;

            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = new Random().Next(1111, 9999).ToString();
            request.Price = totalPrice.ToString(System.Globalization.CultureInfo.InvariantCulture);
            //request.PaidPrice = totalPrice - discountAmount;
            request.PaidPrice = totalPrice.ToString(System.Globalization.CultureInfo.InvariantCulture);


            //request.Price = "15";//checkoutHeaderDto.OrderTotal.ToString();
            //request.PaidPrice = "15";//checkoutHeaderDto.OrderTotal.ToString();
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = "B67832";
            request.BasketId = checkoutHeaderDto.CartHeaderId.ToString();
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = $"{checkoutHeaderDto.FirstName} {checkoutHeaderDto.LastName}";
            paymentCard.CardNumber = checkoutHeaderDto.CardNumber;
            paymentCard.ExpireMonth = checkoutHeaderDto.ExpiryMonth;
            paymentCard.ExpireYear = checkoutHeaderDto.ExpiryYear;
            paymentCard.Cvc = checkoutHeaderDto.CVV;
            paymentCard.RegisterCard = 0;
            paymentCard.CardAlias = "Inveon";
            request.PaymentCard = paymentCard;

            Buyer buyer = new Buyer();
            //buyer.Id = cartDto.CartHeader.UserId;
            buyer.Id = "BY789";
            buyer.Name = checkoutHeaderDto.FirstName;
            buyer.Surname = checkoutHeaderDto.LastName;
            buyer.GsmNumber = checkoutHeaderDto.Phone;
            buyer.Email = checkoutHeaderDto.Email;
            buyer.IdentityNumber = "74300864791";
            buyer.LastLoginDate = "2015-10-05 12:43:35";
            buyer.RegistrationDate = "2013-04-21 15:12:09";
            buyer.RegistrationAddress = "Inveon Headquarters, Levent Istanbul";
            buyer.Ip = "85.34.78.112";
            buyer.City = "Istanbul";
            buyer.Country = "Turkey";
            buyer.ZipCode = "34732";
            request.Buyer = buyer;

            Address shippingAddress = new Address();
            shippingAddress.ContactName = $"{checkoutHeaderDto.FirstName} {checkoutHeaderDto.LastName}"; ;
            shippingAddress.City = "Istanbul";
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = "Inveon Headquarters, Levent Istanbul";
            shippingAddress.ZipCode = "34742";
            request.ShippingAddress = shippingAddress;

            Address billingAddress = new Address();
            billingAddress.ContactName = $"{checkoutHeaderDto.FirstName} {checkoutHeaderDto.LastName}"; ;
            billingAddress.City = "Istanbul";
            billingAddress.Country = "Turkey";
            billingAddress.Description = "Inveon Headquarters, Levent Istanbul";
            billingAddress.ZipCode = "34742";
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();

            // Sepetteki her ürünü teker teker iyzico'nun sipariş detayları kısmına yazdır.
            foreach (CartDetailsDto productDetails in cartDto.CartDetails)
            {
                ProductDto productInfo = productDetails.Product;
                BasketItem basketItem = new BasketItem();
                basketItem.Id = productInfo.ProductId.ToString();
                basketItem.Name = productInfo.Name;
                // basketItem.Category1 = productInfo.CategoryName;
                basketItem.Category1 = "TODO CHANGE CATEGORY";
                basketItem.ItemType = BasketItemType.PHYSICAL.ToString();
                basketItem.Price = (productInfo.Price * productDetails.Count).ToString(System.Globalization.CultureInfo.InvariantCulture);
                basketItems.Add(basketItem);
            }

            request.BasketItems = basketItems;

            return Payment.Create(request, options);
        }
    }
}
