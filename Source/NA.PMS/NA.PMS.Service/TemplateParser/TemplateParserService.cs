using NA.PMS.Model;
using NA.PMS.Repository.TemplateParser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Service.TemplateParser
{
    public class TemplateParserService : ITemplateParserService
    {
        private ITemplateParserRepository _templateParserRepository;
        public TemplateParserService()
        {
            _templateParserRepository = new TemplateParserRepository();

        }

        public string GetParsedHTML<T>(T model, string templateName)
        {
            return _templateParserRepository.GetParsedHTML(model, templateName);
        }

        public string GetGeneratedChallanByRegistrationId(ChallanViewModel model)
        {
            return _templateParserRepository.GetGeneratedChallanByRegistrationId(model);
        }

        public string GetGeneratedChallanByPropertyNo(ChallanViewModel model)
        {
            return _templateParserRepository.GetGeneratedChallanByPropertyNo(model);
        }

        public ChallanViewModel GetGeneratedChallanByChallanId(ChallanViewModel model)
        {
            return _templateParserRepository.GetGeneratedChallanByChallanId(model);
        }


        public PaymentViewModel CreateCustomDemandNote(PaymentViewModel model, string templateName)
        {
            return _templateParserRepository.CreateCustomDemandNote(model, templateName);
        }


        public string GetParsedAndSavedDemandNote(PaymentViewModel model, string templateName)
        {
            return _templateParserRepository.GetParsedAndSavedDemandNote(model, templateName);
        }


        public string GenerateNDCLetter(NDCVeiwModel model, string templateName)
        {
            return _templateParserRepository.GenerateNDCLetter(model, templateName);
        }


        public string GetGeneratedLedgerByRId(PaymentViewModel model)
        {
            //return _templateParserRepository.GetGeneratedLedgerByRId(model);
            return _templateParserRepository.GetGeneratedLedgerByRIdNew(model);
        }

        public ChallanViewModel GenerateChallanForPaymentById(ChallanViewModel model)
        {
            return _templateParserRepository.GenerateChallanForPaymentById(model);
        }


        public string GetGeneratedChallanForEmployee(ChallanViewModel model)
        {
            return _templateParserRepository.GetGeneratedChallanForEmployee(model);
        }
    }
}
