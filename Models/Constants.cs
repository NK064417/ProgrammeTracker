namespace ProgrammeTracker.Models
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string AccountManager = "Account Manager";
        public const string ProjectCoordinator = "Project Coordinator";
        public const string Facilitator = "Facilitator";
        public const string TalentManager = "Talent Manager";
        public const string Payroll = "Payroll";
        public const string Management = "Management";

        public static readonly string[] All =
        [
            Admin, AccountManager, ProjectCoordinator,
            Facilitator, TalentManager, Payroll, Management
        ];
    }

    public static class DeliveryTypes
    {
        public static readonly string[] All = ["Campus", "Online", "Inhouse"];
    }

    public static class ProgrammeStatuses
    {
        public const string Active = "Active";
        public const string NotActive = "Not Active";
        public const string Completed = "Completed";
        public static readonly string[] All = [Active, NotActive, Completed];
    }

    public static class DeliverableStatuses
    {
        public const string Pending = "Pending";
        public const string InProgress = "In Progress";
        public const string Complete = "Complete";
        public const string Overdue = "Overdue";
        public static readonly string[] All = [Pending, InProgress, Complete, Overdue];
    }

    public static class Provinces
    {
        public static readonly string[] All =
        [
            "Gauteng", "Western Cape", "Eastern Cape", "KwaZulu-Natal",
            "Limpopo", "Mpumalanga", "North West", "Free State", "Northern Cape"
        ];
    }

    public static class ReportingFrequencies
    {
        public static readonly string[] All = ["Weekly", "Monthly", "Quarterly"];
    }

    public static class AppSettingKeys
    {
        public const string SmtpHost = "SmtpHost";
        public const string SmtpPort = "SmtpPort";
        public const string SmtpUsername = "SmtpUsername";
        public const string SmtpPassword = "SmtpPassword";
        public const string SmtpFromAddress = "SmtpFromAddress";
        public const string NotifyEnabled = "NotifyEnabled";
        public const string CheckIntervalSeconds = "CheckIntervalSeconds";
    }
}
