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
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRageMath;
using CollisionLayers = Sandbox.Engine.Physics.MyPhysics.CollisionLayers;

namespace Controllable_Fighters.Data.Scripts.ControllableFighters
{
    internal class ControllableShip : MyEntity
    {
        public virtual void Init(IMyModContext ModContext, string model)
        {
            Init(null, model, null, null);

            if (model == null || model == "")
                Flags &= ~EntityFlags.Visible;

            Save = false;
            NeedsWorldMatrix = true;

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
            PhysicsSettings settings = MyAPIGateway.Physics.CreateSettingsForPhysics(
                this,
                WorldMatrix,
                Vector3.Zero,
                linearDamping: 0,
                angularDamping: 0,
                collisionLayer: CollisionLayers.DefaultCollisionLayer,
                rigidBodyFlags: RigidBodyFlag.RBF_UNLOCKED_SPEEDS,
                isPhantom: false,
                mass: new ModAPIMass(PositionComp.LocalAABB.Volume(), 10000, Vector3.Zero, new Matrix(48531.0f, -1320.0f, 0.0f, -1320.0f, 256608.0f, 0.0f, 0.0f, 0.0f, 211333.0f))
                );

            //settings.DetectorColliderCallback += HitCallback;
            //settings.Entity.Flags |= EntityFlags.IsGamePrunningStructureObject;
            MyAPIGateway.Physics.CreateBoxPhysics(settings, PositionComp.LocalAABB.HalfExtents, 0);

            Physics.Enabled = true;
            Physics.Activate();
        }

        private void HitCallback(IMyEntity entity, bool arg2)
        {
            
        }
    }
}
