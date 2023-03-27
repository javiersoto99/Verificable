Create Database InscripcionesBrDb
GO

USE [InscripcionesBrDb]
GO


CREATE TABLE [dbo].[Adquirente](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Rut] [varchar](12) NOT NULL,
    [Nombre] [varchar](50) NOT NULL,
    [Porcentaje] [float],
    [Porcentaje_Na] [int] NOT NULL,
    CONSTRAINT [PK_Adquirente] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

CREATE TABLE [dbo].[Enajenante](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Rut] [varchar](12) NOT NULL,
    [Nombre] [varchar](50) NOT NULL,
    [Porcentaje] [float],
    [Porcentaje_Na] [int] NOT NULL,
    CONSTRAINT [PK_Enajenante] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO


CREATE TABLE [dbo].[Comuna](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Nombre] [varchar](50) NOT NULL,
    CONSTRAINT [PK_Comuna] PRIMARY KEY CLUSTERED ([Id] ASC)
)


CREATE TABLE [dbo].[Rol](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Fk_comuna] [int] NOT NULL,
    [Manzana] [int] NOT NULL,
    [Predio] [int] NOT NULL,
    CONSTRAINT [PK_Rol] PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY (Fk_comuna) REFERENCES Comuna(Id)
)
GO


CREATE TABLE [dbo].[Inscripcion](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Numero_atencion] [int] NOT NULL,
    [Cne] [int] NOT NULL,
    [Fojas] [int] NOT NULL,
    [Creacion] [date] NOT NULL,
    [Fk_rol] [int] NOT NULL,
    CONSTRAINT [PK_Inscripcion] PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY (Fk_rol) REFERENCES Rol(Id)
)
GO


CREATE TABLE [dbo].[Inscripcion_Adquirente](
    [Fk_inscripcion] [int] NOT NULL,
    [Fk_adquirente] [int] NOT NULL,
    PRIMARY KEY (Fk_inscripcion, Fk_adquirente),
    FOREIGN KEY (Fk_inscripcion) REFERENCES Inscripcion(Id),
    FOREIGN KEY (Fk_adquirente) REFERENCES Adquirente(Id)
)
GO


CREATE TABLE [dbo].[Inscripcion_Enajenante](
    [Fk_inscripcion] [int] NOT NULL,
    [Fk_enajenante] [int] NOT NULL,
    PRIMARY KEY (Fk_inscripcion, Fk_enajenante),
    FOREIGN KEY (Fk_inscripcion) REFERENCES Inscripcion(Id),
    FOREIGN KEY (Fk_enajenante) REFERENCES Enajenante(Id)
)


CREATE TABLE [dbo].[Multipropietario](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Fojas] [int] NOT NULL,
    [Ano_inscripcion] [int] NOT NULL,
    [Fk_numero_inscripcion] [int] NOT NULL,
    [Vigencia_inicial] [int] NOT NULL,
    [Vigencia_final] [int],
    CONSTRAINT [PK_Multipropietario] PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY (Fk_numero_inscripcion) REFERENCES Inscripcion(Id)
)
GO


--Poblar BBDD:

USE [InscripcionesBrDb]
GO

-- Inscripcion 1
INSERT [dbo].[Adquirente] ([Rut], [Nombre], [Porcentaje], [Porcentaje_Na]) VALUES ('20654321-0', 'Juan Perez', 50, 0)
INSERT [dbo].[Adquirente] ([Rut], [Nombre], [Porcentaje], [Porcentaje_Na]) VALUES ('20654322-1', 'Tomas Ruiz', 20, 0)
INSERT [dbo].[Adquirente] ([Rut], [Nombre], [Porcentaje], [Porcentaje_Na]) VALUES ('20654323-2', 'Maria Lopez', 30, 0)

INSERT [dbo].[Enajenante] ([Rut], [Nombre], [Porcentaje], [Porcentaje_Na]) VALUES ('20654324-3', 'Luis Varela', 10, 0)
INSERT [dbo].[Enajenante] ([Rut], [Nombre], [Porcentaje], [Porcentaje_Na]) VALUES ('20654325-4', 'Pedro Gomez', 90, 0)

INSERT [dbo].[Comuna] ([Nombre]) VALUES ('Las Condes')

INSERT [dbo].[Rol] ([Fk_comuna], [Manzana], [Predio]) VALUES (1, 12, 13)

INSERT [dbo].[Inscripcion] ([Numero_atencion], [Cne], [Fojas], [Creacion], [Fk_rol]) VALUES (1, 0, 0, '2019-01-01', 1)

INSERT [dbo].[Inscripcion_Adquirente] ([Fk_inscripcion], [Fk_adquirente]) VALUES (1, 1)
INSERT [dbo].[Inscripcion_Adquirente] ([Fk_inscripcion], [Fk_adquirente]) VALUES (1, 2)
INSERT [dbo].[Inscripcion_Adquirente] ([Fk_inscripcion], [Fk_adquirente]) VALUES (1, 3)

INSERT [dbo].[Inscripcion_Enajenante] ([Fk_inscripcion], [Fk_enajenante]) VALUES (1, 1)
INSERT [dbo].[Inscripcion_Enajenante] ([Fk_inscripcion], [Fk_enajenante]) VALUES (1, 2)

-- Inscripcion 2
INSERT [dbo].[Adquirente] ([Rut], [Nombre], [Porcentaje], [Porcentaje_Na]) VALUES ('20654326-5', 'Elena Saez', NULL, 1)

INSERT [dbo].[Enajenante] ([Rut], [Nombre], [Porcentaje], [Porcentaje_Na]) VALUES ('20654327-6', 'Jose Soto', 100, 0)

INSERT [dbo].[Comuna] ([Nombre]) VALUES ('Vitacura')

INSERT [dbo].[Rol] ([Fk_comuna], [Manzana], [Predio]) VALUES (2, 10, 20)

INSERT [dbo].[Inscripcion] ([Numero_atencion], [Cne], [Fojas], [Creacion], [Fk_rol]) VALUES (2, 0, 0, '2020-01-01', 2)

INSERT [dbo].[Inscripcion_Adquirente] ([Fk_inscripcion], [Fk_adquirente]) VALUES (2, 4)

INSERT [dbo].[Inscripcion_Enajenante] ([Fk_inscripcion], [Fk_enajenante]) VALUES (2, 3)




