using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Profound.Controllers
{
    public class PaymentController
    {

        [HttpGet("{courseId}/purchase")]
        public  Purchase(int courseId)
        {
            var payment = new Payment { courseId, userId };
            _dataRepository.PostPurchase(courseId);
        }
    }
}
