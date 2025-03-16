﻿using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Features.ForeignAgent.Commands.AddNewForeignCv { }

public class AddNewForeignCvRequestHandler : IRequestHandler<AddAddNewForeignCvRequest, int>
{
    readonly ICVRepository _unitOfWork;
    readonly IForeignAgentRepository _foreignAgent;
    readonly ICVStatusRepository _cvStatus;
    readonly IAttachmentRepository _attachmentRepository;
    readonly IPreviousEmploymentsRepository _PreviousEmployments;
    readonly IMapper mapper;
    readonly ICandidateSkillsRepository _candidateSkillsRepository;

    public AddNewForeignCvRequestHandler(ICVRepository unitOfWork, IMapper mapper,
        ILogger<AddCountryHandler> logger, IForeignAgentRepository foreignAgent, ICVStatusRepository cvStatus, IAttachmentRepository attachmentRepository, IPreviousEmploymentsRepository previousEmployments, ICandidateSkillsRepository candidateSkillsRepository)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.mapper = mapper;
        _foreignAgent = foreignAgent;
        _cvStatus = cvStatus;
        _attachmentRepository = attachmentRepository;
        _PreviousEmployments = previousEmployments;
        _candidateSkillsRepository = candidateSkillsRepository;
    }

    public async Task<int> Handle(AddAddNewForeignCvRequest request, CancellationToken cancellationToken)
    {
        var newForeignAgentCV = await _unitOfWork.AddAsync(mapper.Map<CV>(request.cv));
        var foreign = await _foreignAgent.GetByIdAsync(request.foreignAgentId);
        var status = await _cvStatus.GetByIdAsync(request.cvStatusId);
        //Add HRPool
        await _unitOfWork.AddHRPool(newForeignAgentCV, foreign, status, request.foreignAgentUserId);

        //add PreviousEmployments
        if (request.previousEmployment != null)
        {
            if (request.previousEmployment.Count > 0)
            {
                await _PreviousEmployments.AddPreviousEmployments
                                           (newForeignAgentCV, request.previousEmployment, request.foreignAgentUserId);
            }
        }

        //add attachments
        if (request.cvAttachments.Count > 0)
        {
            var attachmentResult = await _attachmentRepository.AddAttachment(request.cvAttachments, request.foreignAgentUserId);
            await _attachmentRepository.AddCVAttachment(newForeignAgentCV, attachmentResult, request.foreignAgentUserId);
        }

        //Add skills
        if (request.Skills is not null && request.Skills.Length > 0)
        {
            await _candidateSkillsRepository.AddCVCandidateSkills(newForeignAgentCV, request.Skills, request.foreignAgentUserId);
        }

        return newForeignAgentCV.Id;
    }
}

