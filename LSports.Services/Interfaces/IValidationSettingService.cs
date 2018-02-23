using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSports.Framework.Models.CustomClasses;
using LSports.Services.ViewModels;

namespace LSports.Services.Interfaces
{
    public interface IValidationSettingService
    {
        IList<ValidationSettingViewModel> GetValidationSettingsForProduct(int? productId, FilterObject filter);
    }
}
