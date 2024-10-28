using Photon.Deterministic;

namespace Quantum
{
    public unsafe class MovementSystem : SystemMainThreadFilter<MovementSystem.Filter>, ISignalOnPlayerDataSet, ISignalOnPlayerDisconnected
    {
        public struct Filter
        {
            public EntityRef Entity;
            public CharacterController3D* CharacterController;
            public Transform3D* Transform;
            public PlayerLink* Link;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            //Game Session data
            GameSession* gameSession = f.Unsafe.GetPointerSingleton<GameSession>();

            if( gameSession == null )
                return;

            if (gameSession->State != GameState.Playing)
                return;

            var input = f.GetPlayerInput(filter.Link->Player);

            var inputVector = new FPVector2((FP)input->DirectionX / 10, (FP)input->DirectionY / 10);

            //Anti Cheat
            if (inputVector.SqrMagnitude > 1)
                inputVector = inputVector.Normalized;

            // Move forward
            filter.CharacterController->Move(f, filter.Entity, inputVector.XOY);

            //Jump
            if (input->Jump.WasPressed)
                filter.CharacterController->Jump(f);

            //Look in the movement dir
            if (inputVector.SqrMagnitude != default)
            {
                filter.Transform->Rotation = FPQuaternion.Lerp(filter.Transform->Rotation, FPQuaternion.LookRotation(inputVector.XOY), f.DeltaTime * 10);
            }

            //Respawn if we fall too low
            if (filter.Transform->Position.Y < -7)
            {
                filter.Transform->Position = GetSpawnPosition(filter.Link->Player, f.PlayerCount);
            }

            Physics3D.HitCollection3D hitCollection = f.Physics3D.OverlapShape(filter.Transform->Position, FPQuaternion.Identity, Shape3D.CreateSphere(1));
        
            for(int i = 0; i < hitCollection.Count; i++)
            {
                if (hitCollection[i].IsTrigger)
                    gameSession->State = GameState.GameOver;
            }
        }

        public void OnPlayerDataSet(Frame f, PlayerRef player)
        {
            var data = f.GetPlayerData(player);

            var prototypeEntity = f.FindAsset<EntityPrototype>(data.CharacterPrototype.Id);
            var createEntity = f.Create(prototypeEntity);

            if (f.Unsafe.TryGetPointer<PlayerLink>(createEntity, out var playerLink))
            {
                playerLink->Player = player;
            }

            if (f.Unsafe.TryGetPointer<Transform3D>(createEntity, out var transform))
            {
                transform->Position = GetSpawnPosition(player, f.PlayerCount);
            }
        }

        FPVector3 GetSpawnPosition(int playerNumber, int playerCount)
        {
            int widthOfAllPlayer = playerCount * 2;

            return new FPVector3((playerNumber * 2) + 1 - widthOfAllPlayer / 2, 0, 0);
        }

        public void OnPlayerDisconnected(Frame f, PlayerRef player)
        {
            foreach (var playerLink in f.GetComponentIterator<PlayerLink>())
            {
                if (playerLink.Component.Player != player)
                    continue;

                f.Destroy(playerLink.Entity);
            }
        }
    }
}
