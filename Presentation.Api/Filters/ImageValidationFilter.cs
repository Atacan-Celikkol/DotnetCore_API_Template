using Core.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using SixLabors.ImageSharp;
using System.Linq;

namespace Presentation.Api.Filters
{
    public class ImageValidationFilterAttribute : ActionFilterAttribute
    {
        public ControlType ControlType { get; set; }
        public int MininumHeight { get; set; }
        public int MinimumWidth { get; set; }
        public float Ratio { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var formImage = context.HttpContext.Request.Form.Files.FirstOrDefault();
            if (formImage == null)
            {
                throw new NoImageException();
            }
            var image = Image.Load(formImage.OpenReadStream());
            var ratio = (float)image.Width / image.Height;

            if (MininumHeight > 0 && MininumHeight > image.Height)
            {
                throw new ImageValidationException();
            }

            if (MinimumWidth > 0 && MinimumWidth > image.Width)
            {
                throw new ImageValidationException();
            }

            if (ControlType == ControlType.FixedResolution && (image.Width != Width || image.Height != image.Height))
            {
                throw new ImageValidationException();
            }

            if (ControlType == ControlType.FixedRatio && ratio != Ratio)
            {
                throw new ImageValidationException();
            }

            base.OnActionExecuting(context);
        }
    }

    public enum ControlType
    {
        OnlyMinimums,
        FixedResolution,
        FixedRatio
    }
}
