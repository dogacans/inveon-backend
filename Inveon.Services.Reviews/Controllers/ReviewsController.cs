using System.Security.Claims;
using Inveon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inveon.Services.Reviews.Controllers;

[ApiController]
[Route("api/")]
public class ReviewController : ControllerBase
{
    private readonly ReviewRepository _reviewRepository;

    public ReviewController(ReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    [Authorize]
    [HttpPost("addReview")]
    public IActionResult AddReview([FromBody] ReviewDto reviewDto)
    {
        ClaimsPrincipal currentUser = this.User;
        string userId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
        
        Review review = new Review();
        review.Comment = reviewDto.Comment;
        review.Rating = reviewDto.Rating;
        review.ProductId = reviewDto.ProductId;
        review.UserId = userId;
        review.Timestamp = DateTime.UtcNow;
        
        try
        {
            _reviewRepository.AddReview(review);
            return Ok(review);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("reviews/{productId}")]
    public IActionResult GetReviews(int productId)
    {
        var reviews = _reviewRepository.GetReviewsForProduct(productId);
        return Ok(reviews);
    }
}