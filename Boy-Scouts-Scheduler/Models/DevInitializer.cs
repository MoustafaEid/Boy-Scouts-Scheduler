﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Boy_Scouts_Scheduler.Models
{
    public class DevInitializer : DropCreateDatabaseIfModelChanges<SchedulingContext>
    {
        protected override void Seed(SchedulingContext context)
        {
            context.Seed();
        }
    }
}