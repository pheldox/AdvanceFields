using AdvanceFields.Models;
using System.Collections.Generic;

namespace AdvanceFields.Services
{
    public interface ITranslation: IServiceBase
    {

        public void SaveTranslation(RqTranslate rqTranslate);
        
        public List<RqTranslate> LoadTranslation();
        

    }
}
