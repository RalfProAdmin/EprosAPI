USE [EprosCareersDB]
GO
/****** Object:  Table [dbo].[JobPostings]    Script Date: 15-04-2026 19:31:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[JobPostings]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[JobPostings](
	[JobId] [int] IDENTITY(1,1) NOT NULL,
	[PostedByUserId] [int] NOT NULL,
	[Title] [nvarchar](200) NOT NULL,
	[Department] [nvarchar](200) NULL,
	[DescriptionHtml] [nvarchar](max) NULL,
	[MinExperiences] [int] NOT NULL,
	[MaxExperiences] [int] NOT NULL,
	[SalaryRange] [nvarchar](100) NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[IsActive] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[JobId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 15-04-2026 19:31:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Roles](
	[RoleID] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [varchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[UserProfiles]    Script Date: 15-04-2026 19:31:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserProfiles]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[UserProfiles](
	[ProfileId] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[CurrentRole] [nvarchar](100) NULL,
	[TotalExperience] [decimal](4, 2) NULL,
	[Summary] [nvarchar](max) NULL,
	[ResumeUrl] [nvarchar](max) NULL,
	[KeySkills] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[ProfileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Users]    Script Date: 15-04-2026 19:31:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[PasswordHash] [nvarchar](max) NOT NULL,
	[RoleId] [int] NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[IsActive] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__JobPostin__MinEx__5535A963]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[JobPostings] ADD  DEFAULT ((0)) FOR [MinExperiences]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__JobPostin__Creat__5629CD9C]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[JobPostings] ADD  DEFAULT (getdate()) FOR [CreatedAt]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__JobPostin__IsAct__571DF1D5]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[JobPostings] ADD  DEFAULT ((1)) FOR [IsActive]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Users__CreatedAt__4D94879B]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [CreatedAt]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Users__IsActive__4E88ABD4]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((1)) FOR [IsActive]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__JobPostin__Poste__5441852A]') AND parent_object_id = OBJECT_ID(N'[dbo].[JobPostings]'))
ALTER TABLE [dbo].[JobPostings]  WITH CHECK ADD FOREIGN KEY([PostedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__UserProfi__UserI__5165187F]') AND parent_object_id = OBJECT_ID(N'[dbo].[UserProfiles]'))
ALTER TABLE [dbo].[UserProfiles]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserId])
ON DELETE CASCADE
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__Users__RoleId__4CA06362]') AND parent_object_id = OBJECT_ID(N'[dbo].[Users]'))
ALTER TABLE [dbo].[Users]  WITH CHECK ADD FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([RoleID])
GO
