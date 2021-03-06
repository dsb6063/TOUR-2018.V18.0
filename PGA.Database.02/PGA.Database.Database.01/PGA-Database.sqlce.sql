-- Script Date: 12/16/2015 4:54 PM  - ErikEJ.SqlCeScripting version 3.5.2.56
-- Database information:
-- Locale Identifier: 1033
-- Encryption Mode: Platform Default
-- Case Sensitive: False
-- Database: C:\Users\Daryl-Win-8\Documents\Visual Studio 2013\Projects\PGA.DatabaseManager\Database\PGA.sdf
-- ServerVersion: 4.0.8876.1
-- DatabaseSize: 192 KB
-- Created: 8/16/2015 1:44 PM

-- User Table information:
-- Number of tables: 10
-- CommandStack: 54 row(s)
-- GlobalSettings: 1 row(s)
-- Logs: 0 row(s)
-- PointCloud: 19 row(s)
-- Polyline: 0 row(s)
-- PolylineChildren: 0 row(s)
-- PolylineDWGS: 18 row(s)
-- Settings: 1 row(s)
-- SourceDrawingInfo: 0 row(s)
-- Tasks: 26 row(s)

CREATE TABLE [Tasks] (
  [Id] int IDENTITY (27,1) NOT NULL
, [ProjectID] int NOT NULL
, [TimeStarted] datetime NULL
, [TimeEnded] datetime NULL
, [IsCancelled] bit NULL
, [IsPaused] bit NULL
);
GO
CREATE TABLE [SourceDrawingInfo] (
  [Id] int IDENTITY (1,1) NOT NULL
, [DrawingName] nvarchar(50) NULL
, [DrawingPath] nvarchar(50) NULL
, [DrawingGui] nvarchar(50) NULL
, [DestinationPath] nvarchar(50) NULL
, [TimeStampFile] nchar(10) NULL
, [TimeStampProcessess] nchar(10) NULL
, [LastSaveTime] nchar(10) NULL
);
GO
CREATE TABLE [Settings] (
  [Id] int IDENTITY (4,1) NOT NULL
, [GlobalPointFilePath] nvarchar(200) NULL
, [GlobalPolylineFilePath] nvarchar(200) NULL
, [TemplateDWG] nvarchar(200) NULL
, [SeverPath] nvarchar(200) NULL
, [CourseName] nvarchar(50) NULL
, [DateStamp] datetime NULL
, [StartTime] datetime NULL
, [EndTime] datetime NULL
, [DestinationFolder] nvarchar(200) NULL
, [CreateDXF] bit NULL
, [DestDXFFolder] nvarchar(200) NULL
, [ProjectName] nvarchar(250) NULL
, [ProjectCity] nvarchar(250) NULL
, [ProjectState] nvarchar(250) NULL
, [ProjectDate] datetime NULL
, [ProjectCreator] nvarchar(250) NULL
, [ProjectManager] nvarchar(250) NULL
, [ProjectSurveyor] nvarchar(250) NULL
);
GO
CREATE TABLE [PolylineDWGS] (
  [Id] int IDENTITY (109,1) NOT NULL
, [DrawingName] nvarchar(50) NULL
, [DrawingGUI] nvarchar(50) NULL
, [SourcePath] nvarchar(200) NULL
, [DestinationPath] nvarchar(200) NULL
, [DateStamp] datetime NULL
, [DateLoaded] datetime NULL
);
GO
CREATE TABLE [PolylineChildren] (
  [Id] int IDENTITY (1,1) NOT NULL
, [PolylineParentID] bigint NOT NULL
, [PolylineChildrenID] nvarchar(50) NULL
, [DateStamp] datetime NULL
, [PolylineID] bigint NULL
);
GO
CREATE TABLE [Polyline] (
  [Id] int IDENTITY (1,1) NOT NULL
, [DrawingName] nchar(50) NULL
, [DrawingID] nchar(50) NOT NULL
, [Handle] nchar(50) NOT NULL
, [ObejctId] nchar(50) NOT NULL
, [Level] nchar(50) NOT NULL
, [Nodes] nchar(50) NOT NULL
, [Layer] nchar(50) NOT NULL
, [Area] nchar(50) NOT NULL
, [Length] nchar(50) NOT NULL
, [Geometry] nchar(200) NOT NULL
);
GO
CREATE TABLE [PointCloud] (
  [Id] int IDENTITY (115,1) NOT NULL
, [DrawingName] nvarchar(50) NULL
, [DrawingGUI] nvarchar(50) NULL
, [SourcePath] nvarchar(200) NULL
, [DestinationPath] nvarchar(200) NULL
, [DateStamp] datetime NULL
, [DateLoaded] datetime NULL
);
GO
CREATE TABLE [Logs] (
  [Id] int IDENTITY (1,1) NOT NULL
, [DateStamp] nchar(10) NOT NULL
, [Issue] nchar(200) NOT NULL
, [DrawingID] nchar(50) NULL
);
GO
CREATE TABLE [GlobalSettings] (
  [Id] int IDENTITY (2,1) NOT NULL
, [PointsPath] nvarchar(200) NULL
, [PolylinePath] nvarchar(200) NULL
, [SourceDWG] nvarchar(200) NULL
, [ServerName] nvarchar(100) NULL
, [ServerPath] nvarchar(100) NULL
, [EmailAddress] nvarchar(100) NULL
, [EmailServer] nvarchar(100) NULL
, [EmailPort] nvarchar(100) NULL
, [EmailUserName] nvarchar(100) NULL
, [EmailPassword] nvarchar(100) NULL
, [IsEmailSecured] bit DEFAULT NULL NULL
, [IsAttachment] bit DEFAULT NULL NULL
);
GO
CREATE TABLE [CommandStack] (
  [Id] int IDENTITY (326,1) NOT NULL
, [Commands] nvarchar(500) NULL
, [Function] nvarchar(100) NULL
, [IsInvoked] bit NULL
, [IsRunning] bit NULL
, [IsCompleted] bit NULL
, [HasError] bit NULL
, [TimeStarted] datetime NULL
, [TimeCompleted] datetime NULL
, [SelectionSetID] bigint NULL
, [SourceDWGID] bigint NULL
, [SourceDWGGui] bigint NULL
);
GO
SET IDENTITY_INSERT [Tasks] ON;
GO
INSERT INTO [Tasks] ([Id],[ProjectID],[TimeStarted],[TimeEnded],[IsCancelled],[IsPaused]) VALUES (1,23,NULL,NULL,NULL,NULL);
GO
INSERT INTO [Tasks] ([Id],[ProjectID],[TimeStarted],[TimeEnded],[IsCancelled],[IsPaused]) VALUES (2,31,NULL,NULL,NULL,NULL);
GO
INSERT INTO [Tasks] ([Id],[ProjectID],[TimeStarted],[TimeEnded],[IsCancelled],[IsPaused]) VALUES (3,32,NULL,NULL,NULL,NULL);
GO
INSERT INTO [Tasks] ([Id],[ProjectID],[TimeStarted],[TimeEnded],[IsCancelled],[IsPaused]) VALUES (4,33,NULL,NULL,NULL,NULL);
GO
INSERT INTO [Tasks] ([Id],[ProjectID],[TimeStarted],[TimeEnded],[IsCancelled],[IsPaused]) VALUES (5,34,NULL,NULL,NULL,NULL);
GO
INSERT INTO [Tasks] ([Id],[ProjectID],[TimeStarted],[TimeEnded],[IsCancelled],[IsPaused]) VALUES (6,35,NULL,NULL,NULL,NULL);
GO
INSERT INTO [Tasks] ([Id],[ProjectID],[TimeStarted],[TimeEnded],[IsCancelled],[IsPaused]) VALUES (7,36,NULL,NULL,NULL,NULL);
GO
INSERT INTO [Tasks] ([Id],[ProjectID],[TimeStarted],[TimeEnded],[IsCancelled],[IsPaused]) VALUES (8,37,NULL,NULL,NULL,NULL);
GO
INSERT INTO [Tasks] ([Id],[ProjectID],[TimeStarted],[TimeEnded],[IsCancelled],[IsPaused]) VALUES (9,38,NULL,NULL,NULL,NULL);
GO
INSERT INTO [Tasks] ([Id],[ProjectID],[TimeStarted],[TimeEnded],[IsCancelled],[IsPaused]) VALUES (10,39,NULL,NULL,NULL,NULL);
GO
INSERT INTO [Tasks] ([Id],[ProjectID],[TimeStarted],[TimeEnded],[IsCancelled],[IsPaused]) VALUES (11,40,NULL,NULL,NULL,NULL);
GO
INSERT INTO [Tasks] ([Id],[ProjectID],[TimeStarted],[TimeEnded],[IsCancelled],[IsPaused]) VALUES (12,41,NULL,NULL,NULL,NULL);
GO
INSERT INTO [Tasks] ([Id],[ProjectID],[TimeStarted],[TimeEnded],[IsCancelled],[IsPaused]) VALUES (13,42,NULL,NULL,NULL,NULL);
GO
INSERT INTO [Tasks] ([Id],[ProjectID],[TimeStarted],[TimeEnded],[IsCancelled],[IsPaused]) VALUES (14,43,NULL,NULL,NULL,NULL);
GO
INSERT INTO [Tasks] ([Id],[ProjectID],[TimeStarted],[TimeEnded],[IsCancelled],[IsPaused]) VALUES (15,43,NULL,NULL,NULL,NULL);
GO
INSERT INTO [Tasks] ([Id],[ProjectID],[TimeStarted],[TimeEnded],[IsCancelled],[IsPaused]) VALUES (16,45,NULL,NULL,NULL,NULL);
GO
INSERT INTO [Tasks] ([Id],[ProjectID],[TimeStarted],[TimeEnded],[IsCancelled],[IsPaused]) VALUES (17,0,{ts '2015-09-02 03:43:37.000'},NULL,0,1);
GO
INSERT INTO [Tasks] ([Id],[ProjectID],[TimeStarted],[TimeEnded],[IsCancelled],[IsPaused]) VALUES (18,46,NULL,NULL,NULL,NULL);
GO
INSERT INTO [Tasks] ([Id],[ProjectID],[TimeStarted],[TimeEnded],[IsCancelled],[IsPaused]) VALUES (19,0,{ts '2015-09-02 03:47:59.000'},NULL,0,1);
GO
INSERT INTO [Tasks] ([Id],[ProjectID],[TimeStarted],[TimeEnded],[IsCancelled],[IsPaused]) VALUES (20,47,NULL,NULL,NULL,NULL);
GO
INSERT INTO [Tasks] ([Id],[ProjectID],[TimeStarted],[TimeEnded],[IsCancelled],[IsPaused]) VALUES (21,1,NULL,NULL,NULL,NULL);
GO
INSERT INTO [Tasks] ([Id],[ProjectID],[TimeStarted],[TimeEnded],[IsCancelled],[IsPaused]) VALUES (22,0,{ts '2015-09-06 03:02:03.000'},NULL,0,1);
GO
INSERT INTO [Tasks] ([Id],[ProjectID],[TimeStarted],[TimeEnded],[IsCancelled],[IsPaused]) VALUES (23,2,NULL,NULL,NULL,NULL);
GO
INSERT INTO [Tasks] ([Id],[ProjectID],[TimeStarted],[TimeEnded],[IsCancelled],[IsPaused]) VALUES (24,0,{ts '2015-09-06 04:14:38.000'},NULL,0,1);
GO
INSERT INTO [Tasks] ([Id],[ProjectID],[TimeStarted],[TimeEnded],[IsCancelled],[IsPaused]) VALUES (25,3,NULL,NULL,NULL,NULL);
GO
INSERT INTO [Tasks] ([Id],[ProjectID],[TimeStarted],[TimeEnded],[IsCancelled],[IsPaused]) VALUES (26,0,{ts '2015-09-06 05:59:36.000'},NULL,0,1);
GO
SET IDENTITY_INSERT [Tasks] OFF;
GO
SET IDENTITY_INSERT [SourceDrawingInfo] OFF;
GO
SET IDENTITY_INSERT [Settings] ON;
GO
INSERT INTO [Settings] ([Id],[GlobalPointFilePath],[GlobalPolylineFilePath],[TemplateDWG],[SeverPath],[CourseName],[DateStamp],[StartTime],[EndTime],[DestinationFolder],[CreateDXF],[DestDXFFolder],[ProjectName],[ProjectCity],[ProjectState],[ProjectDate],[ProjectCreator],[ProjectManager],[ProjectSurveyor]) VALUES (3,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM',N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons',N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons',N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons',N'pga',{ts '2015-09-06 05:59:36.000'},NULL,NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\',1,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\',N'D',N'D',NULL,{ts '2015-09-06 05:59:36.000'},N'D',N'D',N'D');
GO
SET IDENTITY_INSERT [Settings] OFF;
GO
SET IDENTITY_INSERT [PolylineDWGS] ON;
GO
INSERT INTO [PolylineDWGS] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (91,N'RTJ-GRAND-NATIONAL01.DWG',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PolylineDWGS] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (92,N'RTJ-GRAND-NATIONAL02.DWG',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PolylineDWGS] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (93,N'RTJ-GRAND-NATIONAL03.DWG',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PolylineDWGS] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (94,N'RTJ-GRAND-NATIONAL04.DWG',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PolylineDWGS] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (95,N'RTJ-GRAND-NATIONAL05.DWG',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PolylineDWGS] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (96,N'RTJ-GRAND-NATIONAL06.DWG',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PolylineDWGS] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (97,N'RTJ-GRAND-NATIONAL07.DWG',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PolylineDWGS] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (98,N'RTJ-GRAND-NATIONAL08.DWG',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PolylineDWGS] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (99,N'RTJ-GRAND-NATIONAL09.DWG',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PolylineDWGS] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (100,N'RTJ-GRAND-NATIONAL10.DWG',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PolylineDWGS] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (101,N'RTJ-GRAND-NATIONAL11.DWG',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PolylineDWGS] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (102,N'RTJ-GRAND-NATIONAL12.DWG',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PolylineDWGS] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (103,N'RTJ-GRAND-NATIONAL13.DWG',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PolylineDWGS] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (104,N'RTJ-GRAND-NATIONAL14.DWG',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PolylineDWGS] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (105,N'RTJ-GRAND-NATIONAL15.DWG',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PolylineDWGS] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (106,N'RTJ-GRAND-NATIONAL16.DWG',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PolylineDWGS] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (107,N'RTJ-GRAND-NATIONAL17.DWG',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PolylineDWGS] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (108,N'RTJ-GRAND-NATIONAL18.DWG',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
SET IDENTITY_INSERT [PolylineDWGS] OFF;
GO
SET IDENTITY_INSERT [PolylineChildren] OFF;
GO
SET IDENTITY_INSERT [Polyline] OFF;
GO
SET IDENTITY_INSERT [PointCloud] ON;
GO
INSERT INTO [PointCloud] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (96,N'all-holes-dtm.txt',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PointCloud] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (97,N'dtm01.txt',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PointCloud] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (98,N'dtm02.txt',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PointCloud] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (99,N'dtm03.txt',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PointCloud] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (100,N'dtm04.txt',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PointCloud] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (101,N'dtm05.txt',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PointCloud] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (102,N'dtm06.txt',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PointCloud] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (103,N'dtm07.txt',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PointCloud] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (104,N'dtm08.txt',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PointCloud] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (105,N'dtm09.txt',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PointCloud] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (106,N'dtm10.txt',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PointCloud] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (107,N'dtm11.txt',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PointCloud] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (108,N'dtm12.txt',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PointCloud] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (109,N'dtm13.txt',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PointCloud] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (110,N'dtm14.txt',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PointCloud] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (111,N'dtm15.txt',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PointCloud] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (112,N'dtm16.txt',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PointCloud] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (113,N'dtm17.txt',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
INSERT INTO [PointCloud] ([Id],[DrawingName],[DrawingGUI],[SourcePath],[DestinationPath],[DateStamp],[DateLoaded]) VALUES (114,N'dtm18.txt',NULL,N'C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM',N'',{ts '2015-09-06 05:59:36.000'},{ts '2015-09-06 05:59:36.000'});
GO
SET IDENTITY_INSERT [PointCloud] OFF;
GO
SET IDENTITY_INSERT [Logs] OFF;
GO
SET IDENTITY_INSERT [GlobalSettings] ON;
GO
INSERT INTO [GlobalSettings] ([Id],[PointsPath],[PolylinePath],[SourceDWG],[ServerName],[ServerPath],[EmailAddress],[EmailServer],[EmailPort],[EmailUserName],[EmailPassword],[IsEmailSecured],[IsAttachment]) VALUES (1,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
SET IDENTITY_INSERT [GlobalSettings] OFF;
GO
SET IDENTITY_INSERT [CommandStack] ON;
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (272,N'01,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm01.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL01.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE01,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (273,N'02,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm02.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL02.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE02,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (274,N'03,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm03.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL03.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE03,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (275,N'04,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm04.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL04.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE04,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (276,N'05,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm05.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL05.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE05,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (277,N'06,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm06.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL06.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE06,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (278,N'07,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm07.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL07.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE07,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (279,N'08,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm08.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL08.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE08,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (280,N'09,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm09.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL09.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE09,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (281,N'10,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm10.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL10.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE10,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (282,N'11,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm11.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL11.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE11,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (283,N'12,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm12.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL12.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE12,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (284,N'13,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm13.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL13.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE13,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (285,N'14,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm14.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL14.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE14,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (286,N'15,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm15.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL15.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE15,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (287,N'16,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm16.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL16.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE16,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (288,N'17,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm17.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL17.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE17,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (289,N'18,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm18.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL18.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE18,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (290,N'01,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm01.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL01.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE01,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (291,N'02,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm02.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL02.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE02,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (292,N'03,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm03.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL03.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE03,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (293,N'04,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm04.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL04.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE04,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (294,N'05,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm05.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL05.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE05,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (295,N'06,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm06.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL06.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE06,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (296,N'07,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm07.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL07.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE07,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (297,N'08,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm08.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL08.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE08,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (298,N'09,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm09.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL09.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE09,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (299,N'10,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm10.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL10.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE10,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (300,N'11,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm11.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL11.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE11,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (301,N'12,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm12.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL12.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE12,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (302,N'13,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm13.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL13.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE13,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (303,N'14,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm14.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL14.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE14,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (304,N'15,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm15.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL15.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE15,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (305,N'16,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm16.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL16.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE16,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (306,N'17,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm17.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL17.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE17,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (307,N'18,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm18.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL18.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE18,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (308,N'01,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm01.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL01.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE01,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (309,N'02,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm02.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL02.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE02,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (310,N'03,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm03.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL03.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE03,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (311,N'04,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm04.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL04.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE04,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (312,N'05,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm05.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL05.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE05,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (313,N'06,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm06.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL06.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE06,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (314,N'07,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm07.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL07.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE07,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (315,N'08,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm08.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL08.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE08,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (316,N'09,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm09.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL09.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE09,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (317,N'10,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm10.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL10.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE10,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (318,N'11,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm11.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL11.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE11,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (319,N'12,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm12.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL12.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE12,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (320,N'13,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm13.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL13.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE13,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (321,N'14,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm14.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL14.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE14,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (322,N'15,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm15.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL15.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE15,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (323,N'16,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm16.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL16.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE16,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (324,N'17,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm17.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL17.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE17,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
INSERT INTO [CommandStack] ([Id],[Commands],[Function],[IsInvoked],[IsRunning],[IsCompleted],[HasError],[TimeStarted],[TimeCompleted],[SelectionSetID],[SourceDWGID],[SourceDWGGui]) VALUES (325,N'18,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\DTM\dtm18.txt,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\__RTJ-Grand-National-Closed-CAD-Polygons\RTJ-GRAND-NATIONAL18.DWG,C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\Points\HOLE18,9/6/2015 5:59:36 AM',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
GO
SET IDENTITY_INSERT [CommandStack] OFF;
GO
ALTER TABLE [Tasks] ADD CONSTRAINT [PK_Tasks] PRIMARY KEY ([Id]);
GO
ALTER TABLE [SourceDrawingInfo] ADD CONSTRAINT [PK_SourceDrawingInfoS] PRIMARY KEY ([Id]);
GO
ALTER TABLE [Settings] ADD CONSTRAINT [PK_Settings] PRIMARY KEY ([Id]);
GO
ALTER TABLE [PolylineDWGS] ADD CONSTRAINT [PK_PolylineDWGS] PRIMARY KEY ([Id]);
GO
ALTER TABLE [PolylineChildren] ADD CONSTRAINT [PK_PolylineChildren] PRIMARY KEY ([Id]);
GO
ALTER TABLE [Polyline] ADD CONSTRAINT [PK_Polyline] PRIMARY KEY ([Id]);
GO
ALTER TABLE [PointCloud] ADD CONSTRAINT [PK_PointCloud] PRIMARY KEY ([Id]);
GO
ALTER TABLE [Logs] ADD CONSTRAINT [PK_Logs] PRIMARY KEY ([Id]);
GO
ALTER TABLE [GlobalSettings] ADD CONSTRAINT [PK_GlobalSettings] PRIMARY KEY ([Id]);
GO
ALTER TABLE [CommandStack] ADD CONSTRAINT [PK_CommandStack] PRIMARY KEY ([Id]);
GO

