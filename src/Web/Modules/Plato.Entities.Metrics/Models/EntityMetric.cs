﻿using System;
using System.Data;
using Plato.Entities.Models;
using PlatoCore.Abstractions;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Models.Users;

namespace Plato.Entities.Metrics.Models
{

    public class EntityMetric : IDbModel
    {

        public int Id { get; set; }
        
        public int EntityId { get; set; }
        
        public ISimpleEntity Entity { get; private set; }

        public string IpV4Address { get; set; }

        public string IpV6Address { get; set; }

        public string UserAgent { get; set; }

        public int CreatedUserId { get; set; }

        public ISimpleUser CreatedBy { get; set; } = new SimpleUser();

        public DateTimeOffset? CreatedDate { get; set; }

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("EntityId"))
                EntityId = Convert.ToInt32(dr["EntityId"]);

            if (EntityId > 0)
            {
                Entity = new SimpleEntity()
                {
                    Id = EntityId
                };
                if (dr.ColumnIsNotNull("FeatureId"))
                    Entity.FeatureId = Convert.ToInt32(dr["FeatureId"]);
                if (dr.ColumnIsNotNull("ModuleId"))
                    Entity.ModuleId = Convert.ToString(dr["ModuleId"]);
                if (dr.ColumnIsNotNull("Title"))
                    Entity.Title = Convert.ToString(dr["Title"]);
                if (dr.ColumnIsNotNull("Alias"))
                    Entity.Alias = Convert.ToString(dr["Alias"]);
            }
       
            if (dr.ColumnIsNotNull("IpV4Address"))
                IpV4Address = Convert.ToString(dr["IpV4Address"]);

            if (dr.ColumnIsNotNull("IpV6Address"))
                IpV6Address = Convert.ToString(dr["IpV6Address"]);

            if (dr.ColumnIsNotNull("UserAgent"))
                UserAgent = Convert.ToString(dr["UserAgent"]);

            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (CreatedUserId > 0)
            {
                CreatedBy = new SimpleUser()
                {
                    Id = CreatedUserId
                };
                if (dr.ColumnIsNotNull("UserName"))
                    CreatedBy.UserName = Convert.ToString(dr["UserName"]);
                if (dr.ColumnIsNotNull("DisplayName"))
                    CreatedBy.DisplayName = Convert.ToString(dr["DisplayName"]);
                if (dr.ColumnIsNotNull("Alias"))
                    CreatedBy.Alias = Convert.ToString(dr["Alias"]);
                if (dr.ColumnIsNotNull("PhotoUrl"))
                    CreatedBy.PhotoUrl = Convert.ToString(dr["PhotoUrl"]);
                if (dr.ColumnIsNotNull("PhotoColor"))
                    CreatedBy.PhotoColor = Convert.ToString(dr["PhotoColor"]);
            }

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = (DateTimeOffset)dr["CreatedDate"];

        }

    }


}
