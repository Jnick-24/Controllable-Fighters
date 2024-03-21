using Sandbox.Engine.Utils;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.ModAPI;
using VRage.Utils;
using VRageMath;
using static VRage.Game.MyObjectBuilder_ControllerSchemaDefinition;
using Controllable_Fighters.Data.Scripts.Debug;
using Sandbox.Game.Entities.Character;
using VRage.Input;
using VRage;
using Sandbox.Engine.Physics;
using Controllable_Fighters.Data.Scripts.ControllableFighters.PlaneParts;

namespace Controllable_Fighters.Data.Scripts.ControllableFighters
{
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
    public class CameraControl : MySessionComponentBase
    {
        const float Sensitivity = 0.1f;

        ControllablePlane ShipEntity = new ControllablePlane();
        bool IsControllingShip = false;
        bool EnableDebug = false;
        MySpectatorCameraController CameraController = null;

        #region Base Methods

        public override void LoadData()
        {
            if (MyAPIGateway.Utilities.IsDedicated)
                return;
            MyAPIGateway.Session.CameraController.IsInFirstPersonView = false;

            ShipEntity.Init(ModContext);
        }

        public override void UpdateBeforeSimulation()
        {
            if (!ShipEntity.Physics?.IsActive ?? false)
            {
                ShipEntity.Physics.Enabled = true;
                ShipEntity.Physics.Activate();
            }

            ShipEntity.Update(EnableDebug);

            MyAPIGateway.Utilities.ShowNotification($"Controlling: {IsControllingShip} | Throttle: {Math.Round(ShipEntity.Throttle * 100, 0)}", 1000 / 60);
            MyAPIGateway.Utilities.ShowNotification($"ShipVel: {Math.Round(ShipEntity.Physics.LinearVelocity.Length(), 1)}", 1000 / 60);

            if (CameraController == null || ShipEntity.Physics == null)
                return;

            if (IsControllingShip)
            {
                //ShipEntity.Physics.LinearVelocity += Vector3D.Rotate(MyAPIGateway.Input.GetPositionDelta(), ShipEntity.WorldMatrix);
                //ShipEntity.Physics.AngularVelocity += Vector3D.Rotate(new Vector3(MyAPIGateway.Input.GetRotation() * MyAPIGateway.Input.GetMouseSensitivity(), MyAPIGateway.Input.GetRoll() * 10) * Sensitivity, -ShipEntity.WorldMatrix);
                //ShipEntity.Physics.SetSpeeds(
                //    ShipEntity.Physics.LinearVelocity + Vector3D.Rotate(MyAPIGateway.Input.GetPositionDelta(), ShipEntity.WorldMatrix),
                //    Vector3D.Rotate(new Vector3(MyAPIGateway.Input.GetRotation() * MyAPIGateway.Input.GetMouseSensitivity(), MyAPIGateway.Input.GetRoll() * 10) * Sensitivity, -ShipEntity.WorldMatrix));

                Vector2 rotation = MyAPIGateway.Input.GetRotation();
                ShipEntity.Pitch = rotation.X;
                ShipEntity.Yaw = rotation.Y;
                ShipEntity.Roll = MyAPIGateway.Input.GetRoll();
            }
        }

        public override void HandleInput()
        {
            if (CameraController != null)
            {
                // Prevent micromovements when using movement controls
                CameraController.Position = ShipEntity.PositionComp.GetPosition() + ShipEntity.Physics.LinearVelocity * 1/60f;
            }

            // Toggle IsControllingShip
            if (MyAPIGateway.Input.IsNewKeyPressed(MyKeys.Insert))
                SetControllingShip(!IsControllingShip);
            // Toggle debug
            if (MyAPIGateway.Input.IsNewKeyPressed(MyKeys.Home))
                EnableDebug = !EnableDebug;

            if (IsControllingShip)
            {
                // Exit ship
                if (MyAPIGateway.Input.IsNewKeyPressed(MyKeys.F6))
                    SetControllingShip(false);
                if (MyAPIGateway.Input.IsNewKeyPressed(MyKeys.F7))
                    SetControllingShip(false);
                if (MyAPIGateway.Input.IsNewKeyPressed(MyKeys.F8))
                    SetControllingShip(false);
                if (MyAPIGateway.Input.IsNewKeyPressed(MyKeys.F9))
                    SetControllingShip(false);

                // Throttle
                if (MyAPIGateway.Input.IsKeyPress(MyKeys.Shift))
                    ShipEntity.Throttle += 0.01f;
                if (MyAPIGateway.Input.IsKeyPress(MyKeys.Control))
                    ShipEntity.Throttle -= 0.01f;
            }
        }

        public override void Draw()
        {
            //if (MyAPIGateway.Utilities.IsDedicated || !IsControllingShip)
            //    return;
            if (CameraController == null)
                return;

            CameraController.Position = ShipEntity.PositionComp.GetPosition() + ShipEntity.Physics.LinearVelocity * 1 / 60f;
            CameraController.SetViewMatrix(ShipEntity.GetViewMatrix());
            //CameraController.SetTarget(ShipEntity.PositionComp.GetPosition() + ShipEntity.WorldMatrix.Forward, ShipEntity.PositionComp.GetPosition() + ShipEntity.WorldMatrix.Up);
        }

        #endregion

        public void SetControllingShip(bool value)
        {
            IsControllingShip = value;

            if (IsControllingShip)
            {
                //MyAPIGateway.SpectatorTools.SetTarget(ShipEntity);
                MyAPIGateway.Session.SetCameraController(MyCameraControllerEnum.SpectatorFreeMouse, ShipEntity, Vector3D.Zero);
                CameraController = MyAPIGateway.Session.CameraController as MySpectatorCameraController;
            }
            else
            {
                MyAPIGateway.Session.SetCameraController(MyCameraControllerEnum.Entity, null, Vector3D.Zero);
                CameraController = null;
            }
        }
    }
}
