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





