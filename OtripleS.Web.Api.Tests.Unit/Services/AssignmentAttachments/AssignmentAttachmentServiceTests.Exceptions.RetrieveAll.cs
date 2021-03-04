﻿using System;
using Microsoft.EntityFrameworkCore;
using Moq;
using OtripleS.Web.Api.Models.AssignmentAttachments.Exceptions;
using Xunit;

namespace OtripleS.Web.Api.Tests.Unit.Services.AssignmentAttachments
{
    public partial class AssignmentAttachmentServiceTests
    {
        [Fact]
        public void ShouldThrowDependencyExceptionOnRetrieveAllAssignmentAttachmentsWhenSqlExceptionOccursAndLogIt()
        {
            // given
            var sqlException = GetSqlException();

            var expectedAssignmentAttachmentDependencyException =
                new AssignmentAttachmentDependencyException(sqlException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllAssignmentAttachments())
                    .Throws(sqlException);

            // when . then
            Assert.Throws<AssignmentAttachmentDependencyException>(() =>
                this.assignmentAttachmentService.RetrieveAllAssignmentAttachments());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllAssignmentAttachments(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(expectedAssignmentAttachmentDependencyException))),
                    Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowDependencyExceptionOnRetrieveAllAssignmentAttachmentsWhenDbExceptionOccursAndLogIt()
        {
            // given
            var databaseUpdateException = new DbUpdateException();

            var expectedAttachmentDependencyException =
                new AssignmentAttachmentDependencyException(databaseUpdateException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllAssignmentAttachments())
                    .Throws(databaseUpdateException);

            // when . then
            Assert.Throws<AssignmentAttachmentDependencyException>(() =>
                this.assignmentAttachmentService.RetrieveAllAssignmentAttachments());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllAssignmentAttachments(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedAttachmentDependencyException))),
                    Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllAssignmentAttachmentsWhenExceptionOccursAndLogIt()
        {
            // given
            var exception = new Exception();

            var expectedAssignmentAttachmentServiceException =
                new AssignmentAttachmentServiceException(exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllAssignmentAttachments())
                    .Throws(exception);

            // when . then
            Assert.Throws<AssignmentAttachmentServiceException>(() =>
                this.assignmentAttachmentService.RetrieveAllAssignmentAttachments());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllAssignmentAttachments(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedAssignmentAttachmentServiceException))),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

    }
}
