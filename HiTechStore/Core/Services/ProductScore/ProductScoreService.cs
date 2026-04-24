using HiTechStore.Core.Auth;
using HiTechStore.Core.Exceptions;
using HiTechStore.Core.Services.Authorization;

namespace HiTechStore.Core.Services.ProductScore;


public class ProductScoreService : ServiceBase, IProductScoreService
{
    private IUnitOfWork _unitOfWork;
    public ProductScoreService(
        IAuthorizationService authorizationService,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork
    ) : base(authorizationService, currentUserProvider)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Models.ProductScore> AddScoreForProduct(
         ProdcutScoreCreationServiceDto productScoreCreationDto
     )
    {

        int rate = productScoreCreationDto.Score;
        if (rate < 1 || rate > 5)
        {
            throw new ModelException(
                "In-correct rate",
                "Rate must be an integer greater than 0 and less than 6 (one of [1, 2, 3, 4 , 5])",
                nameof(Models.ProductScore.Score)
            );
        }

        var user = await GetUser();

        // check if is any score registered by this user for this product before
        var score = await _unitOfWork.ProductScores.GetUserScoreForProductAsync(user.Id, productScoreCreationDto.ProductId);
        if (score != null)
        {
            // if exist delete it
            score.Score = productScoreCreationDto.Score;
        }
        else
        {
            // register new one
            score = new Models.ProductScore
            {
                UserId = user.Id,
                ProductId = productScoreCreationDto.ProductId,
                Score = productScoreCreationDto.Score // default score
            };
            await _unitOfWork.ProductScores.AddAsync(score);

        }

        await _unitOfWork.Complete();

        return score;

    }
}
