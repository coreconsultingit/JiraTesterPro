﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiraTesterProData;
using JiraTesterProService;

namespace JiraTesterProTestFixture
{
   
    public  class JiraTestFixtureBase
    {
        protected IServiceProvider _serviceProvider;
        [OneTimeSetUp]
        public void BaseSetUp()
        {
            var servicecollection = new ServiceCollection();
            servicecollection.RegisterDependency(new JiraTesterCommandLineOptions());
            _serviceProvider = BootStrapper.ServiceProvider;
        }
    }
}
