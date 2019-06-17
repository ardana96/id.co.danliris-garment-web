﻿using ExtCore.Data.Abstractions;
using Infrastructure.Domain.Commands;
using Manufactures.Domain.GarmentPreparings;
using Manufactures.Domain.GarmentPreparings.Commands;
using Manufactures.Domain.GarmentPreparings.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Manufactures.Application.GarmentPreparings.CommandHandlers
{
    public class PlaceGarmentPreparingCommandHandler : ICommandHandler<PlaceGarmentPreparingCommand, GarmentPreparing>
    {
        private readonly IGarmentPreparingRepository _garmentPreparingRepository;
        private readonly IGarmentPreparingItemRepository _garmentPreparingItemRepository;
        private readonly IStorage _storage;

        public PlaceGarmentPreparingCommandHandler(IStorage storage)
        {
            _storage = storage;
            _garmentPreparingItemRepository = storage.GetRepository<IGarmentPreparingItemRepository>();
            _garmentPreparingRepository = storage.GetRepository<IGarmentPreparingRepository>();
        }

        public async Task<GarmentPreparing> Handle(PlaceGarmentPreparingCommand request, CancellationToken cancellationToken)
        {
            var garmentPreparing  = _garmentPreparingRepository.Find(o =>
                                    o.UENId == request.UENId &&
                                    o.UENNo == request.UENNo &&
                                    o.UnitId == request.UnitId.Value &&
                                    o.ProcessDate == request.ProcessDate &&
                                    o.RONo == request.RONo &&
                                    o.Article == request.Article &&
                                    o.IsCuttingIn == request.IsCuttingIn).FirstOrDefault();
            List<GarmentPreparingItem> garmentPreparingItem = new List<GarmentPreparingItem>();
            if (garmentPreparing == null)
            {
                garmentPreparing = new GarmentPreparing(Guid.NewGuid(), request.UENId, request.UENNo, request.UnitId, request.ProcessDate, request.RONo,
                        request.Article, request.IsCuttingIn);
                request.Items.Select(x => new GarmentPreparingItem(Guid.NewGuid(), x.UENItemId, x.Product, x.DesignColor, x.Quantity, x.Uom, x.FabricType, x.RemainingQuantity, x.BasicPrice, garmentPreparing.Identity)).ToList()
                    .ForEach(async x => await _garmentPreparingItemRepository.Update(x));
            }

            garmentPreparing.SetModified();

            await _garmentPreparingRepository.Update(garmentPreparing);

            _storage.Save();

            return garmentPreparing;

        }
    }
}
