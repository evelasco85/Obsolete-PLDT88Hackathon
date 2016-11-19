using Pldt.Browser.Api.Database;
using Pldt.Browser.Api.Infrastructure;
using Pldt.Browser.Api.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace Pldt.Browser.Api.Controllers
{
    public class PayMayaController : ApiController
    {

        string _customerUrl = "https://pg-sandbox.paymaya.com/payments/v1/customers";
        string _cardUrl = "https://pg-sandbox.paymaya.com/payments/v1/payment-tokens";
        string _secretKey = "sk-oF33wv5pXp7gvIpHLvCjoNKW2BL48ZxPY6H2Lc8v5mD";
        string _publicKey = "pk-zkronn6BaMpTDvkO2aBGJAZzSmKkz3K6k8t9cDSbwwl";

        IEFRepository _repository;

        public PayMayaController(IEFRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public string CreateCustomer(
            string firstName, string middleName, string lastName,
            string birthday, string sex,
            string line1, string line2,
            string city, string state, string zipCode, string countryCode,
            string phone, string email
            )
        {
            var customerData = new
            {
                firstName,
                middleName,
                lastName,
                sex,
                contact = new
                {
                    phone,
                    email
                },
                billingAddress = new
                {
                    line1,
                    line2,
                    city,
                    state,
                    zipCode,
                    countryCode
                },
                metadata = new { }
            };

            if (_repository.Entities.CustomerRecords.Any(customer => customer.email == email))
                return _repository.Entities.CustomerRecords.Where(customer => customer.email == email).FirstOrDefault().id;

            string jsonInput = new JavaScriptSerializer().Serialize(customerData);
            string jsonOutput = PayMayaGateway
                   .GetInstance()
                   .SendRequest(_secretKey, _customerUrl, jsonInput);
            string id = CustomerService.GetInstance().GetCustomerId(jsonOutput);

            _repository.Entities.CustomerRecords.Add(new CustomerRecord
            {
                id = id,
                jsonData = jsonOutput,
                email = email
            });

            _repository.Entities.SaveChanges();

            return id;
        }

        [HttpPost]
        public string RegisterCreditCard(string customerId,
            string cardNumber,
            string expirationMonth, string expirationYear,
            string cvc)
        {
            var cardData = new
            {
                card = new
                {
                    number = cardNumber,
                    expMonth = expirationMonth,
                    expYear = expirationYear,
                    cvc
                }
            };

            string hash = PayMayaGateway
                .GetInstance()
                .HashGenerator(cardNumber);

            if (_repository.Entities.CardRecords.Any(card => card.hashNumber == hash))
                return _repository.Entities.CardRecords.Where(card => card.hashNumber == hash).FirstOrDefault().paymentTokenId;

            string jsonInput = new JavaScriptSerializer().Serialize(cardData);
            string jsonOutput = PayMayaGateway
                   .GetInstance()
                   .SendRequest(_publicKey, _cardUrl, jsonInput);
            string paymentTokenId = PaymentService.GetInstance().GetPaymentTokenId(jsonOutput);

            _repository.Entities.CardRecords.Add(new CardRecord
            {
                paymentTokenId = paymentTokenId,
                hashNumber = hash
            });

            try
            {
                _repository.Entities.SaveChanges();
            }
            catch
            {
                throw;
            }

            return paymentTokenId;
        }
    }
}
