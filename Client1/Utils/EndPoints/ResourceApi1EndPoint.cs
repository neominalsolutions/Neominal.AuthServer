using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client1.Utils.EndPoints
{
    public class ResourceApi1EndPoint
    {
        public const string ProductUrl = "https://localhost:5003/api/products/list";
        public const string InstructorApproveEndPoint = "https://localhost:5003/api/InstructorsRegisteration/approval";
        public const string InstructorCreateEndPoint = "https://localhost:5003/api/InstructorsRegisteration/create";
        public const string InstructorDenyEndPoint = "https://localhost:5003/api/InstructorsRegisteration/deny";
        public const string  WeatherForecastUrl = "https://localhost:5003/WeatherForecast";
    }
}
