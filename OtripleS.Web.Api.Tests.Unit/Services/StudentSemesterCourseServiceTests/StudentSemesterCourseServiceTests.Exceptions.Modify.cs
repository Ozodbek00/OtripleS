﻿// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE AS LONG AS SOFTWARE FUNDS ARE DONATED TO THE POOR
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using OtripleS.Web.Api.Models.StudentSemesterCourses;
using OtripleS.Web.Api.Models.StudentSemesterCourses.Exceptions;
using Xunit;

namespace OtripleS.Web.Api.Tests.Unit.Services.StudentSemesterCourseServiceTests
{
    public partial class StudentSemesterCourseServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            int randomNegativeNumber = GetNegativeRandomNumber();
            DateTimeOffset randomDateTime = GetRandomDateTime();
            StudentSemesterCourse randomStudentSemesterCourse = CreateRandomStudentSemesterCourse(randomDateTime);
            StudentSemesterCourse someStudentSemesterCourse = randomStudentSemesterCourse;
            someStudentSemesterCourse.CreatedDate = randomDateTime.AddMinutes(randomNegativeNumber);
            SqlException sqlException = GetSqlException();

            var expectedStudentSemesterCourseDependencyException =
                new StudentSemesterCourseDependencyException(sqlException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectStudentSemesterCourseByIdAsync
                (someStudentSemesterCourse.StudentId, someStudentSemesterCourse.SemesterCourseId))
                    .ThrowsAsync(sqlException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime())
                    .Returns(randomDateTime);

            // when
            ValueTask<StudentSemesterCourse> modifyStudentSemesterCourseTask =
                this.studentSemesterCourseService.ModifyStudentSemesterCourseAsync(someStudentSemesterCourse);

            // then
            await Assert.ThrowsAsync<StudentSemesterCourseDependencyException>(() =>
                modifyStudentSemesterCourseTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStudentSemesterCourseByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(expectedStudentSemesterCourseDependencyException))),
                    Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDbUpdateExceptionOccursAndLogItAsync()
        {
            // given
            int randomNegativeNumber = GetNegativeRandomNumber();
            DateTimeOffset randomDateTime = GetRandomDateTime();
            StudentSemesterCourse randomStudentSemesterCourse = CreateRandomStudentSemesterCourse(randomDateTime);
            StudentSemesterCourse someStudentSemesterCourse = randomStudentSemesterCourse;
            someStudentSemesterCourse.CreatedDate = randomDateTime.AddMinutes(randomNegativeNumber);
            var databaseUpdateException = new DbUpdateException();

            var expectedStudentSemesterCourseDependencyException =
                new StudentSemesterCourseDependencyException(databaseUpdateException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectStudentSemesterCourseByIdAsync
                (someStudentSemesterCourse.StudentId, someStudentSemesterCourse.SemesterCourseId))
                    .ThrowsAsync(databaseUpdateException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime())
                    .Returns(randomDateTime);

            // when
            ValueTask<StudentSemesterCourse> modifyStudentSemesterCourseTask =
                this.studentSemesterCourseService.ModifyStudentSemesterCourseAsync(someStudentSemesterCourse);

            // then
            await Assert.ThrowsAsync<StudentSemesterCourseDependencyException>(() =>
                modifyStudentSemesterCourseTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStudentSemesterCourseByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedStudentSemesterCourseDependencyException))),
                    Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
