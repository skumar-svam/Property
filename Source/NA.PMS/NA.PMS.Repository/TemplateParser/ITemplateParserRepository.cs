using NA.PMS.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Repository.TemplateParser
{
    public interface ITemplateParserRepository
    {
        string GetParsedHTML<T>(T model, string templateName);

        string GetGeneratedChallanByRegistrationId(ChallanViewModel model);

        string GetGeneratedChallanByPropertyNo(ChallanViewModel model);

        ChallanViewModel GetGeneratedChallanByChallanId(ChallanViewModel model);

        PaymentViewModel CreateCustomDemandNote(PaymentViewModel model, string templateName);

        string GetParsedAndSavedDemandNote(PaymentViewModel model, string templateName);

        string GenerateNDCLetter(NDCVeiwModel model, string templateName);

        string GetGeneratedLedgerByRId(PaymentViewModel model);

        string GetGeneratedLedgerByRIdNew(PaymentViewModel model);

        ChallanViewModel GenerateChallanForPaymentById(ChallanViewModel model);

        string GetGeneratedChallanForEmployee(ChallanViewModel model);
    }
}
