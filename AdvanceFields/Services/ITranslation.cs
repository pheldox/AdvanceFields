using AdvanceFields.Models;
using System.Collections.Generic;

namespace AdvanceFields.Services
{
    public interface ITranslation: IServiceBase
    {

         void SaveTranslation(RqTranslate rqTranslate);
        
        IEnumerable<RqTranslate> LoadTranslation();
        

    }
}
