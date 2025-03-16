﻿using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Features.LocalAgent.Commands.UpdateLocalUnSelectedCV { }

public class UpdateLocalUnSelectedCVRequestHandler : IRequestHandler<UpdateLocalUnSelectedCVRequest, int>
{
    readonly ILocalSelectedCVRepository _unitOfWork;
    readonly ICVStatusRepository _cvStatus;
    readonly ICVHRPool _ICVHRPoolRepository;
    readonly IMapper mapper;

    public UpdateLocalUnSelectedCVRequestHandler(ILocalSelectedCVRepository unitOfWork, IMapper mapper,
        ILogger<AddLocalSelectedCVRequestHandler> logger, ICVStatusRepository cvStatus, ICVHRPool iCVHRPoolRepository)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.mapper = mapper;
        _cvStatus = cvStatus;
        _ICVHRPoolRepository = iCVHRPoolRepository;
    }

    public async Task<int> Handle(UpdateLocalUnSelectedCVRequest request, CancellationToken cancellationToken)
    {
        var selectedCv = await _unitOfWork.GetAsync(predicate: cv => cv.HRPoolId == request.HRPoolId);

        if (selectedCv.Count < 0 || selectedCv is null)
        {
            throw new NotFoundException(nameof(HRPool), request.HRPoolId);
        }

        selectedCv[0].LocalAgentStatusId = (int)cvStatus.SendToLocal;
        selectedCv[0].LastModifiedById = request.CreatedById;
        selectedCv[0].LastModifiedDate = DateTime.Now;
        //update selected cv in table
        await _unitOfWork.UpdateAsync(selectedCv[0]);

        //update cvHRPools status into sendToLocal
        await UpdateHRPoolStatus(request);

        return selectedCv[0].Id;
    }

    private async Task UpdateHRPoolStatus(UpdateLocalUnSelectedCVRequest request)
    {
        var status = await _cvStatus.GetByIdAsync((int)cvStatus.SendToLocal);
        var getHRPoolById = await _ICVHRPoolRepository.GetByIdAsync((int)request.HRPoolId);

        if (getHRPoolById == null)
        {
            throw new NotFoundException(nameof(HRPool), request.HRPoolId);
        }

        getHRPoolById.LastModifiedById = request.CreatedById;
        getHRPoolById.LastModifiedDate = DateTime.Now;
        getHRPoolById.CVStatus = status;

        await _ICVHRPoolRepository.UpdateAsync(getHRPoolById);
    }
}

