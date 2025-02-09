﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Core.GOAP;

namespace Core.Goals
{
    public class CreatureKilledGoal : GoapGoal
    {
        public override float CostOfPerformingAction { get => 4.1f; }

        public override bool Repeatable { get; } = false;

        private readonly ILogger logger;
        private readonly PlayerReader playerReader;
        private readonly ClassConfiguration classConfig;

        public CreatureKilledGoal(ILogger logger, PlayerReader playerReader, ClassConfiguration classConfig)
        {
            this.logger = logger;
            this.playerReader = playerReader;
            this.classConfig = classConfig;

            AddPrecondition(GoapKey.producedcorpse, true);
            AddPrecondition(GoapKey.consumecorpse, false);

            AddEffect(GoapKey.producedcorpse, false);
            AddEffect(GoapKey.consumecorpse, true);

            if (classConfig.Loot)
            {
                AddEffect(GoapKey.shouldloot, true);

                if(classConfig.Skin)
                {
                    AddEffect(GoapKey.shouldskin, true);
                }
            }
        }

        public override async Task PerformAction()
        {
            logger.LogWarning("------    Safe to consume the kill");
            playerReader.ProduceCorpse();

            SendActionEvent(new ActionEventArgs(GoapKey.producedcorpse, false));
            SendActionEvent(new ActionEventArgs(GoapKey.consumecorpse, true));

            if (classConfig.Loot)
            {
                SendActionEvent(new ActionEventArgs(GoapKey.shouldloot, true));
            }

            await Task.Delay(10);
        }
    }
}
