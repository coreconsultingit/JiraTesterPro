﻿using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiraTesterProData.Extensions;

namespace JiraTesterProService
{
    public interface IJiraTestResultWriter
    {
        Task<IList<JiraTestResultWriterResult>> WriteTestResult(DateTime startTime, IList<JiraTestResult> lstJiraTestResult);
    }
}
