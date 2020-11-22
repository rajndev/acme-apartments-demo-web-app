﻿using PeachGroveApartments.Core.Models;
using System.Collections.Generic;

namespace Peach_Grove_Apartments_Demo_Project.ViewModels
{
    public class FloorPlansViewModel
    {
        public IList<FloorPlan> StudioPlans { get; set; }
        public IList<FloorPlan> OneBedPlans { get; set; }
        public IList<FloorPlan> TwoBedPlans { get; set; }
    }
}