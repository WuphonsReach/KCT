﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kerbal_Construction_Time
{
    public class KCT_TechItem
    {
        public int scienceCost;
        public string techName, techID;
        public double progress;
        private ProtoTechNode protoNode;
        public double BuildRate { get { return (Math.Pow(2, KCT_GameStates.RDUpgrades[1] + 1) / 86400.0); } } //0pts=1day/2sci, 1pt=1/4, 2=1/8, 3=1/16, 4=1/32...n=1/2^(n+1)
        public double TimeLeft { get { return (scienceCost - progress) / BuildRate; } }
        public bool isComplete { get { return progress >= scienceCost; } }

        public KCT_TechItem(RDTech techNode)
        {
            scienceCost = techNode.scienceCost;
            techName = techNode.title;
            techID = techNode.techID;
            progress = 0;
            protoNode = ResearchAndDevelopment.Instance.GetTechState(techID);

            Debug.Log("[KCT] techID = " + techID);
            Debug.Log("[KCT] BuildRate = " + BuildRate);
            Debug.Log("[KCT] TimeLeft = " + TimeLeft);
        }
        
        public KCT_TechItem(string ID, string name, double prog, int sci)
        {
            techID = ID;
            techName = name;
            progress = prog;
            scienceCost = sci;
            protoNode = ResearchAndDevelopment.Instance.GetTechState(techID);
        }

        public void DisableTech()
        {
            protoNode.state = RDTech.State.Unavailable;
            ResearchAndDevelopment.Instance.SetTechState(techID, protoNode);
        }

        public void EnableTech()
        {
            protoNode.state = RDTech.State.Available;
            ResearchAndDevelopment.Instance.SetTechState(techID, protoNode);
        }

        public bool isInList()
        {
            foreach (KCT_TechItem tech in KCT_GameStates.TechList)
            {
                if (tech.techID == this.techID)
                    return true;
            }
            return false;
        }
    }

    public class KCT_TechStorageItem
    {
        [Persistent] string techName, techID;
        [Persistent] int scienceCost;
        [Persistent] double progress;
        public KCT_TechItem ToTechItem()
        {
            KCT_TechItem ret = new KCT_TechItem(techID, techName, progress, scienceCost);
            return ret;
        }
        public KCT_TechStorageItem FromTechItem(KCT_TechItem techItem)
        {
            this.techName = techItem.techName;
            this.techID = techItem.techID;
            this.progress = techItem.progress;
            this.scienceCost = techItem.scienceCost;

            return this;
        }
    }

    public class KCT_TechStorage : ConfigNodeStorage
    {
        [Persistent] List<KCT_TechStorageItem> techBuildList = new List<KCT_TechStorageItem>();
        public override void OnEncodeToConfigNode()
        {
            base.OnEncodeToConfigNode();
            techBuildList.Clear();
            foreach (KCT_TechItem tech in KCT_GameStates.TechList)
            {
                KCT_TechStorageItem tSI = new KCT_TechStorageItem();
                techBuildList.Add(tSI.FromTechItem(tech));
            }
        }

        public override void OnDecodeFromConfigNode()
        {
            base.OnDecodeFromConfigNode();
            KCT_GameStates.TechList.Clear();
            foreach (KCT_TechStorageItem tSI in this.techBuildList)
            {
                KCT_GameStates.TechList.Add(tSI.ToTechItem());
            }
        }
    }
}
/*
Copyright (C) 2014  Michael Marvin, Zachary Eck

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/