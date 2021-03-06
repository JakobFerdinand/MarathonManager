﻿using AutoMapper;
using Core.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using UI.JsonImport.Interfaces;

namespace UI.JsonImport.Services
{
    internal class JsonDeserializer : IJsonDeserializer
    {
        private readonly IMapper _mapper;

        public JsonDeserializer(IMapper mapper) => _mapper = mapper;

        public IEnumerable<Runner> Deserialize(string json)
        {
            var data = JsonConvert.DeserializeObject<IEnumerable<Models.ImportObject>>(json);

            var runners = _mapper.Map<IEnumerable<Runner>>(data);
            return  runners;
        }
    }
}
