﻿// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE AS LONG AS SOFTWARE FUNDS ARE DONATED TO THE POOR
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtripleS.Web.Api.Models.CourseAttachments.Exceptions
{
    public class CourseAttachmentDependencyException : Exception
    {
        public CourseAttachmentDependencyException(Exception innerException)
            : base("Service dependency error occurred, contact support.", innerException) { }
    }
}
