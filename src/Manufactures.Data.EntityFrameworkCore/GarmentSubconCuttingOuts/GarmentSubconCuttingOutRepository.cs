﻿using Infrastructure.Data.EntityFrameworkCore;
using Infrastructure.Data.EntityFrameworkCore.Utilities;
using Manufactures.Domain.GarmentCuttingOuts.ReadModels;
using Manufactures.Domain.GarmentSubconCuttingOuts;
using Manufactures.Domain.GarmentSubconCuttingOuts.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manufactures.Data.EntityFrameworkCore.GarmentSubconCuttingOuts
{
    public class GarmentSubconCuttingOutRepository : AggregateRepostory<GarmentSubconCuttingOut, GarmentCuttingOutReadModel>, IGarmentSubconCuttingOutRepository
    {
        public IQueryable<GarmentCuttingOutReadModel> Read(int page, int size, string order, string keyword, string filter)
        {
            var data = Query;

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            data = QueryHelper<GarmentCuttingOutReadModel>.Filter(data, FilterDictionary);

            List<string> SearchAttributes = new List<string>
            {
                "CutOutNo",
                "UnitCode",
                "RONo",
                "Article",
            };

            data = QueryHelper<GarmentCuttingOutReadModel>.Search(data, SearchAttributes, keyword);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            data = OrderDictionary.Count == 0 ? data.OrderByDescending(o => o.ModifiedDate) : QueryHelper<GarmentCuttingOutReadModel>.Order(data, OrderDictionary);

            data = data.Skip((page - 1) * size).Take(size);

            return data;
        }

        protected override GarmentSubconCuttingOut Map(GarmentCuttingOutReadModel readModel)
        {
            return new GarmentSubconCuttingOut(readModel);
        }
    }
}
