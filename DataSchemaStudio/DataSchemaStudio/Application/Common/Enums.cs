namespace DataSchemaStudio.Application.Common;

public enum NamingLayer
{
    Domain,
    Dto
}

public enum NamingStrategy
{
    None = 0,
    Regex = 1
}

public enum RelationshipTypeEn
{
    None = 1,
    OneToOne = 1,
    OneToMany = 2,
    ManyToOne = 3,
    ManyToMany = 4
}

public enum OperationParameterType
{
    Integer = 1,
    String = 2,
    Boolean = 3,
    Date = 4,
    DateTime = 5,
    Unknown = 6
}

public enum DbProvider
{
    SqlServer = 1,
    MySQL = 2,
    Sqlite = 3
}

public enum DbAuthType
{
    None = 1,
    AzureAD = 2
}

public enum NamingCase
{
    None = 1,
    CamelCase = 2,
    PascalCase = 3
}

