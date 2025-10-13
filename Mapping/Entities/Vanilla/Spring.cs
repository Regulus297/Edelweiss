using System;
using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal abstract class Spring : CSEntityData, IFieldInfoEntity
    {
        public override int Depth(RoomData room, Entity entity) => -8501;
        public override List<float> Justification(RoomData room, Entity entity) => [0.5f, 1.0f];

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override string Texture(RoomData room, Entity entity) => "objects/spring/00";

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("playerCanUse", true);
        }
    }

    internal class SpringUp : Spring
    {
        public override string EntityName => "spring";
        public override bool Rotate(RoomData room, Entity entity, int rotation)
        {
            if (rotation > 0)
            {
                entity.Name = "wallSpringLeft";
            }
            else
            {
                entity.Name = "wallSpringRight";
            }
            return true;
        }
    }

    internal class SpringRight : Spring
    {
        public override string EntityName => "wallSpringRight";
        public override bool Rotate(RoomData room, Entity entity, int rotation)
        {
            if (rotation < 0)
            {
                entity.Name = "spring";
                return true;
            }
            return false;
        }
        public override bool Flip(RoomData room, Entity entity, bool horizontal, bool vertical)
        {
            if (horizontal)
                entity.Name = "wallSpringLeft";
            return horizontal;
        }
        public override float Rotation(RoomData room, Entity entity) => -MathF.PI/2;
    }

    internal class SpringLeft : Spring
    {
        public override string EntityName => "wallSpringLeft";
        public override bool Rotate(RoomData room, Entity entity, int rotation)
        {
            if (rotation > 0)
            {
                entity.Name = "spring";
                return true;
            }
            return false;
        }
        public override bool Flip(RoomData room, Entity entity, bool horizontal, bool vertical)
        {
            if (horizontal)
                entity.Name = "wallSpringRight";
            return horizontal;
        }
        public override float Rotation(RoomData room, Entity entity) => MathF.PI/2;
    }
}