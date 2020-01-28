﻿namespace PlatoCore.Data.Schemas.Abstractions.Builders
{
    public interface IProcedureBuilder : ISchemaBuilderBase
    {

        IProcedureBuilder CreateProcedure(SchemaProcedure procedure);

        IProcedureBuilder DropProcedure(SchemaProcedure procedure);

        IProcedureBuilder AlterProcedure(SchemaProcedure procedure);

        IProcedureBuilder CreateDefaultProcedures(SchemaTable table);

        IProcedureBuilder DropDefaultProcedures(SchemaTable table);

    }

}

