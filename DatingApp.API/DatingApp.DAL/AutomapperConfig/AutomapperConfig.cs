﻿using AutoMapper;
using DatingApp.Domain.DTOs;
using DatingApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.DAL.AutomapperConfig
{
    public class AutomapperConfig: Profile
    {

        public AutomapperConfig()
        {
            // This is a map from AppUser to MemeberDto
            // This is how we configure the PhotoUrl property in MemberDto
            CreateMap<AppUser, MemberDto>()
                .ForMember(destination => destination.PhotoUrl, 
                 options => options.MapFrom(src => src.Photos.FirstOrDefault(x=>x.IsMain).Url));
            CreateMap<Photo, PhotoDto>();
        }


    }
}