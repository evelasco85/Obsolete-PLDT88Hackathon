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
        string _secretKey = "sk-oF33wv5pXp7gvIpHLvCjoNKW2BL48ZxPY6H2Lc8v5mD";
        string _publicKey = "pk-zkronn6BaMpTDvkO2aBGJAZzSmKkz3K6k8t9cDSbwwl";


        string _customerCreationUrl = "https://pg-sandbox.paymaya.com/payments/v1/customers";
        string _cardCreationUrl = "https://pg-sandbox.paymaya.com/payments/v1/payment-tokens";
        string _cardVaultingUrl = "https://pg-sandbox.paymaya.com/payments/v1/customers/{0}/cards";
        string _paymentCreationgUrl = "https://pg-sandbox.paymaya.com/payments/v1/customers/{0}/cards/{1}/payments";


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
                   .SendRequest(_secretKey, _customerCreationUrl, jsonInput);
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
        public string RegisterCard(string customerId,
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
                   .SendRequest(_publicKey, _cardCreationUrl, jsonInput);
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

        [HttpPost]
        public string VaultCard(string customer_id, string paymentTokenId)
        {
            var cardVault = new
            {
                paymentTokenId,
                isDefault = true,
                redirectUrl = new
                {
                    success = "http://shop.server.com/success?id=123",
                    failure = "http://shop.server.com/failure?id=123",
                    cancel = "http://shop.server.com/cancel?id=123"
                }
            };

            CardRecord cardEntry = _repository.Entities.CardRecords
                .Where(card => card.paymentTokenId == paymentTokenId)
                .FirstOrDefault();

            if ((cardEntry != null) && (!string.IsNullOrEmpty(cardEntry.cardTokenId)))
                return cardEntry.cardTokenId;

            string hashNumber = cardEntry.hashNumber;
            string jsonInput = new JavaScriptSerializer().Serialize(cardVault);
            string jsonOutput = PayMayaGateway
                   .GetInstance()
                   .SendRequest(_secretKey,
                   string.Format(_cardVaultingUrl, customer_id),
                   jsonInput);

            CardRecord matchedCard = _repository.Entities.CardRecords.Find(hashNumber);

            try
            {
                if(matchedCard != null)
                {
                    matchedCard.cardTokenId = PaymentService.GetInstance().GetCardTokenId(jsonOutput);
                    _repository.Entities.SaveChanges();
                }
                
            }
            catch
            {
                throw;
            }

            return paymentTokenId;
        }

        [HttpGet]
        public string GetCards(string customer_id)
        {
            string jsonOutput = PayMayaGateway
                   .GetInstance()
                   .GetData(_secretKey,
                   string.Format(_cardVaultingUrl, customer_id));

            return jsonOutput;
        }

        [HttpPost]
        public string CreatePayment(string customer_id, string card_id,
            string amount, string currency
            )
        {
            var amountDetail = new
            {
                totalAmount = new
                {
                    amount,
                    currency
                }
            };

            string jsonInput = new JavaScriptSerializer().Serialize(amountDetail);
            string jsonOutput = PayMayaGateway
                   .GetInstance()
                   .SendRequest(_secretKey,
                   string.Format(_paymentCreationgUrl, customer_id, card_id),
                   jsonInput);

            string paymentStatus = PaymentService.GetInstance().GetPaymentStatus(jsonOutput);

            return paymentStatus;
        }
    }
}
