﻿using MediatR;

namespace App.Application.Features.ForeignAgent.Commands.AddNewForeignCv { }

public class AddAddNewForeignCvRequest : IRequest<int>
{
    public CVDto cv { get; set; }

    public List<Attachment> cvAttachments { get; set; }

    public HRPool cvHRpool { get; set; }

    public List<PreviousEmployment> previousEmployment { get; set; }

    public int foreignAgentId { get; set; }

    public int cvStatusId { get; set; }

    public string foreignAgentUserId { get; set; }

    public int HRPoolId { get; set; }

    public bool? personalImg { get; set; }
    public bool? posterImg { get; set; }
    public bool? passportImg { get; set; }

    public int[] Skills { get; set; }

    public List<SkillSelectedDto> skillList { get; set; }

}

