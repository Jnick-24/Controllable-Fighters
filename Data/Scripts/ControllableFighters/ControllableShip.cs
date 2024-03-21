using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.ModAPI;
using VRageMath;
using CollisionLayers = Sandbox.Engine.Physics.MyPhysics.CollisionLayers;

namespace Controllable_Fighters.Data.Scripts.ControllableFighters
{
    internal class ControllableShip : MyEntity
    {
        public virtual void Init()
        {
            Init(null, "Models\\Weapons\\Projectile_Missile.mwm", null, null);
            Save = false;
            NeedsWorldMatrix = false;

            //Flags |= EntityFlags.Visible;
            //Flags |= EntityFlags.Near;
            //Flags |= EntityFlags.Sync;
            //Flags |= EntityFlags.NeedsDraw;
            PositionComp.LocalAABB = new BoundingBox(-Vector3.Half, Vector3.Half);

            WorldMatrix = MatrixD.CreateWorld(Vector3D.Zero, Vector3D.Forward, Vector3D.Up);

            MyEntities.Add(this, true);

            CreatePhysics();
        }

        private void CreatePhysics()
        {
            PhysicsSettings settings = new PhysicsSettings();
            settings.RigidBodyFlags |= RigidBodyFlag.RBF_UNLOCKED_SPEEDS;
            settings.CollisionLayer |= CollisionLayers.DefaultCollisionLayer;
            settings.IsPhantom = false;
            //settings.RigidBodyFlags |= RigidBodyFlag.RBF_DOUBLED_KINEMATIC;
            settings.DetectorColliderCallback += HitCallback;
            settings.Entity = this;
            settings.WorldMatrix = WorldMatrix;
            settings.Mass = new ModAPIMass(PositionComp.LocalAABB.Volume(), 10000, Vector3.Zero, new Matrix(48531.0f, -1320.0f, 0.0f, -1320.0f, 256608.0f, 0.0f, 0.0f, 0.0f, 211333.0f));
            //settings.Entity.Flags |= EntityFlags.IsGamePrunningStructureObject;
            MyAPIGateway.Physics.CreateBoxPhysics(settings, Vector3.Half, 0);
            Physics.Enabled = true;
            Physics.Activate();
        }

        private void HitCallback(IMyEntity entity, bool arg2)
        {
            
        }
    }
}
