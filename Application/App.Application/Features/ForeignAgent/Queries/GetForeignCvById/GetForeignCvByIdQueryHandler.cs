﻿using AutoMapper;
using MediatR;

namespace App.Application.Features.ForeignAgent.Queries.GetForeignCvById { }


public class GetForeignCvByIdQueryHandler : IRequestHandler<GetForeignCvByIdQuery, ForeignAgentHRPoolDto>
{
    private readonly ICVRepository _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICandidateSkillsRepository _candidateSkillsRepository;

    public GetForeignCvByIdQueryHandler(ICVRepository unitOfWork, IMapper mapper, ICandidateSkillsRepository candidateSkillsRepository)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _candidateSkillsRepository = candidateSkillsRepository;
    }

    public async Task<ForeignAgentHRPoolDto> Handle(GetForeignCvByIdQuery request,
            CancellationToken cancellationToken)
    {
        var foregnAgentCv = await _unitOfWork.GetForeignCvById(request.cvId);

        //get cv attachments list & prevous employment list
        var Cvattachments = await _unitOfWork.GetCvAttachmentByCvId(request.cvId);

        //get cv prevous employment
        var cvPreviousEmployment = await _unitOfWork.GetCvPreviousEmploymentByCvId(request.cvId);

        //add attachments & PreviousEmployment into foregnAgentCv

        //get candidate skils
        var skills = await _candidateSkillsRepository.GetCVCandidateSkills(request.cvId);
        var allSkills = new List<SkillSelectedDto>();
        int[] skillList = null;
        if (skills.Count > 0)
        {
            skillList = new int[skills.Count];
            for (int i = 0; i < skills.Count; i++)
            {
                var skillId = skills[i].CandidateSkillsId;
                skillList[i] = skillId;
            }

            //fill skills
            foreach (var skill in skills)
            {
                var skillobj = new SkillSelectedDto();
                skillobj.Text = skill.CandidateSkills.SkillEnglish;
                skillobj.TextِArabic = skill.CandidateSkills.SkillArabic;
                skillobj.Value = skill.CandidateSkills.SkillArabic;

                allSkills.Add(skillobj);
            }
        }

        ForeignAgentHRPoolDto foreignAgentHRPoolDto = new();
        foreignAgentHRPoolDto.ForeignAgent = foregnAgentCv.ForeignAgent;
        foreignAgentHRPoolDto.CV = foregnAgentCv.CV;
        foreignAgentHRPoolDto.CVStatus = foregnAgentCv.CVStatus;
        foreignAgentHRPoolDto.cvAttachments = Cvattachments;
        foreignAgentHRPoolDto.previousEmployment = cvPreviousEmployment;
        foreignAgentHRPoolDto.cvHRpool = foregnAgentCv;
        foreignAgentHRPoolDto.Skills = skillList;

        foreignAgentHRPoolDto.skillList = allSkills;

        return foreignAgentHRPoolDto;
    }
}
