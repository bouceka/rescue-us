using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnimalService.Entities;

namespace AnimalService.Helpers
{
    public class EnumHelper
    {
        public static Status EnumParse(string value, Status defaultStatus)
        {
            if (!Enum.TryParse(value, out Status animalStatus))
            {
                return defaultStatus;
            }
            return animalStatus;
        }
    }
}