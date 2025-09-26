// using Castle.Components.DictionaryAdapter.Xml;

// using HiTechStore.Core;

// using Microsoft.AspNetCore.Mvc;

// namespace HiTechStore.Controllers;

// [ApiController]
// [Route("categories/{categoryId}/[controller]")]
// public class ComponentsController : ControllerBase
// {

//     private IUnitOfWork _unitOfWork { get; }

//     public ComponentsController(IUnitOfWork unitOfWork) : base()
//     {
//         _unitOfWork = unitOfWork;
//     }

//     [HttpGet]
//     public IResult GetComponents(int categoryId)
//     {
//         _unitOfWork.Compo.GetComponentType();
//     }

//     [HttpGet("{id}")]
//     public IResult GetComponentModels(int id)
//     {

//     }

//     [HttpPost("{id}")]
//     public ActionResult CreateComponentModel()
//     {

//     }
// }