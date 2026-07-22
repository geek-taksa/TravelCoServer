CREATE TABLE TravelCo_Users (
  Id            INT IDENTITY(1,1) PRIMARY KEY,
  Username      NVARCHAR(50)  NOT NULL,
  Email         NVARCHAR(256) NOT NULL UNIQUE,
  PasswordHash  NVARCHAR(256) NOT NULL,
  PasswordSalt  NVARCHAR(256) NOT NULL,
  Role          NVARCHAR(20)  NOT NULL DEFAULT 'user',
  IsLocked      BIT           NOT NULL DEFAULT 0,
  CanShare      BIT           NOT NULL DEFAULT 1,
  CreatedAt     DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE TABLE TravelCo_UserContinents (
  UserId INT NOT NULL,
  Continent NVARCHAR(50) NOT NULL,
  PRIMARY KEY (UserId, Continent),
  CONSTRAINT FK_UserContinents_Users
   FOREIGN KEY (UserId) REFERENCES TravelCo_Users(Id) ON DELETE CASCADE
);

CREATE TABLE TravelCo_UserLanguages (
  UserId INT NOT NULL,
  LanguageName NVARCHAR(50) NOT NULL,
  Level NVARCHAR(20) NOT NULL,
  PRIMARY KEY (UserId, LanguageName),
  CONSTRAINT FK_UserContinents_Users
    FOREIGN KEY (UserId) REFERENCES TravelCo_Users(Id) ON DELETE CASCADE
);

CREATE TABLE TravelCo_Countries (
  Code        NVARCHAR(3)   PRIMARY KEY,
  Name        NVARCHAR(100) NOT NULL,
  Capital     NVARCHAR(100) NULL,
  Region      NVARCHAR(50)  NULL,
  Population  BIGINT        NULL,
  Area        FLOAT         NULL,
  Flag        NVARCHAR(255) NULL
);

CREATE TABLE TravelCo_CountryLanguages (
  CountryCode  NVARCHAR(3)  NOT NULL,
  Language     NVARCHAR(50) NOT NULL,
  PRIMARY KEY (CountryCode, Language),
  CONSTRAINT FK_CountryLanguages_Countries
      FOREIGN KEY (CountryCode) REFERENCES TravelCo_Countries(Code) ON DELETE CASCADE
);

CREATE TABLE TravelCo_CountryCurrencies (
  CountryCode NVARCHAR(3) NOT NULL,
  Currency NVARCHAR(50) NOT NULL,
  PRIMARY KEY (CountryCode, Currency),
  CONSTRAINT FK_CountryCurrencies_Countries
    FOREIGN KEY (CountryCode) REFERENCES TravelCo_Countries(Code) ON DELETE CASCADe
);

CREATE TABLE TravelCo_UserLists (
  UserId       INT           NOT NULL,
  CountryCode  NVARCHAR(3)   NOT NULL,
  ListType     NVARCHAR(20)  NOT NULL,
  AddedAt      DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
  PRIMARY KEY (UserId, CountryCode),
  CONSTRAINT FK_UserLists_Users
      FOREIGN KEY (UserId) REFERENCES TravelCo_Users(Id) ON DELETE CASCADE,
  CONSTRAINT FK_UserLists_Countries
      FOREIGN KEY (CountryCode) REFERENCES TravelCo_Countries(Code) ON DELETE CASCADE
);

CREATE TABLE TravelCo_Shares (
  Id           INT IDENTITY(1,1) PRIMARY KEY,
  UserId       INT           NOT NULL,
  CountryCode  NVARCHAR(3)   NOT NULL,
  Type         NVARCHAR(20)  NOT NULL,
  Title        NVARCHAR(150) NOT NULL,
  Body         NVARCHAR(MAX) NOT NULL,
  CreatedAt    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
  CONSTRAINT FK_Shares_Users
      FOREIGN KEY (UserId) REFERENCES TravelCo_Users(Id) ON DELETE CASCADE,
  CONSTRAINT FK_Shares_Countries
      FOREIGN KEY (CountryCode) REFERENCES TravelCo_Countries(Code) ON DELETE CASCADE
);

CREATE TABLE TravelCo_Quizzes (
  Id INT IDENTITY(1,1) PRIMARY KEY,
  Title NVARCHAR(100) NOT NULL,
  TimeLimitSec INT NOT NULL
);

CREATE TABLE TravelCo_QuizQuestions (
  Id INT IDENTITY(1,1) PRIMARY KEY,
  QuizId INT NOT NULL,
  Prompt NVARCHAR(300) NOT NULL,
  AnswerIndex INT NOT NULL,
  CONSTRAINT FK_QuizQuestions_Quizzes
    FOREIGN KEY (QuizId) REFERENCES TravelCo_Quizzes(Id) ON DELETE CASCADE
);

CREATE TABLE TravelCo_QuizOptions (
  Id INT IDENTITY(1,1) PRIMARY KEY,
  QuestionId INT NOT NULL,
  OptionText NVARCHAR(150) NOT NULL,
  OrderIndex INT NOT NULL,
  CONSTRAINT FK_QuizOptions_QuizQuestions
    FOREIGN KEY (QuestionId) REFERENCES TravelCo_QuizQuestions(Id) ON DELETE CASCADE
);

CREATE TABLE TravelCo_QuizResults (
  Id INT IDENTITY(1,1) PRIMARY KEY,
  UserId INT NOT NULL,
  QuizId INT NOT NULL,
  Score INT NOT NULL,
  Points INT NOT NULL,
  PlayedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
  CONSTRAINT FK_QuizResults_Users
    FOREIGN KEY (UserId) REFERENCES TravelCo_Users(Id) ON DELETE CASCADE,
  CONSTRAINT FK_QuizResults_Quizzes
    FOREIGN KEY (QuizId) REFERENCES TravelCo_Quizzes(Id) ON DELETE CASCADE
);

CREATE TABLE TravelCo_LoginEvents (
  Id INT IDENTITY(1,1) PRIMARY KEY,
  UserId INT NOT NULL,
  LoggedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
  CONSTRAINT FK_LoginEvents_Users
    FOREIGN KEY (UserId) REFERENCES TravelCo_Users(Id) ON DELETE CASCADE
);