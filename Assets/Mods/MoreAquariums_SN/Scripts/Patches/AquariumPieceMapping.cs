namespace DaftAppleGames.MoreAquariums.Patches
{
    /// <summary>
    /// Simple pair class to track mapping between Base.Piece and AquariumType
    /// </summary>
    public class AquariumPieceMapping
    {
        private AquariumType _aquariumType;
        private Base.Piece _basePiece;
        private bool _isGhostPatched;
        private bool _isBasePatched;
        
        internal AquariumType AquariumType => _aquariumType;
        internal Base.Piece BasePiece => _basePiece;
        internal bool IsGhostPatched => _isGhostPatched;
        internal bool IsBasePatched => _isBasePatched;
        internal AquariumPieceMapping(AquariumType aquariumType, Base.Piece basePiece)
        {
            _aquariumType = aquariumType;
            _basePiece = basePiece;
            _isGhostPatched = false;
            _isBasePatched = false;
        }

        internal void SetGhostPatched()
        {
            _isGhostPatched = true;
        }

        internal void SetBasePatched()
        {
            _isBasePatched = true;
        }
    }
}