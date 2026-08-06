using System.Collections.Generic;

namespace DaftAppleGames.MoreAquariums.Patches
{
    /// <summary>
    /// Simple List class to maintain a list of Aquarium to Base.Piece mappings
    /// </summary>
    public class AquariumPieceMappingList
    {
        private List<AquariumPieceMapping> _aquariumPieceMappingList;

        /// <summary>
        /// Default constructor
        /// </summary>
        internal AquariumPieceMappingList()
        {
            _aquariumPieceMappingList = new List<AquariumPieceMapping>();
        }

        /// <summary>
        /// Adds the given AquariumType and CellType as a new AquariumCellMapping to the list
        /// if it doesn't already exist
        /// </summary>
        internal void AddAquariumPieceMapping(AquariumType aquariumType, Base.Piece basePiece)
        {
            AquariumPieceMapping newMapping = new AquariumPieceMapping(aquariumType, basePiece);
            AddAquariumPieceMapping(newMapping);
        }

        /// <summary>
        /// Adds the given AquariumPieceMapping to the list, if it doesn't already exist
        /// </summary>
        internal void AddAquariumPieceMapping(AquariumPieceMapping newAquariumPieceMapping)
        {
            if (!_aquariumPieceMappingList.Contains(newAquariumPieceMapping))
            {
                _aquariumPieceMappingList.Add(newAquariumPieceMapping);                
            }
        }

        /// <summary>
        /// Removes the given AquariumPieceMapping from the list, if it exists
        /// </summary>
        internal void RemoveAquariumPieceMapping(AquariumPieceMapping existingAquariumPieceMapping)
        {
            if (_aquariumPieceMappingList.Contains(existingAquariumPieceMapping))
            {
                _aquariumPieceMappingList.Remove(existingAquariumPieceMapping);
            }
        }

        internal AquariumType GetAquariumType(Base.Piece basePiece)
        {
            foreach (AquariumPieceMapping aquariumPieceMapping in _aquariumPieceMappingList)
            {
                if (aquariumPieceMapping.BasePiece == basePiece)
                {
                    return aquariumPieceMapping.AquariumType;
                }
            }
            return AquariumType.None;
        }
        
        /// <summary>
        /// Returns true if we have patched the ghost prefab for the given aquarium type
        /// </summary>
        internal bool IsGhostPatched(AquariumType aquariumType)
        {
            foreach (AquariumPieceMapping aquariumPieceMapping in _aquariumPieceMappingList)
            {
                if (aquariumPieceMapping.AquariumType == aquariumType)
                {
                    return aquariumPieceMapping.IsGhostPatched;
                }
            }
            return false;
        }
        
        /// <summary>
        /// Returns true if we have patched the ghost prefab for the given base piece
        /// </summary>
        internal bool IsGhostPatched(Base.Piece basePiece)
        {
            foreach (AquariumPieceMapping aquariumPieceMapping in _aquariumPieceMappingList)
            {
                if (aquariumPieceMapping.BasePiece == basePiece)
                {
                    return aquariumPieceMapping.IsGhostPatched;
                }
            }
            return false;
        }
        
        /// <summary>
        /// Override
        /// Returns true if we have patched the base prefab for the given base piece
        /// </summary>
        internal bool IsBasePatched(AquariumType aquariumType)
        {
            foreach (AquariumPieceMapping aquariumPieceMapping in _aquariumPieceMappingList)
            {
                if (aquariumPieceMapping.AquariumType == aquariumType)
                {
                    return aquariumPieceMapping.IsBasePatched;
                }
            }
            return false;
        }

        /// <summary>
        /// Override
        /// Returns true if we have patched the base prefab for the given base piece
        /// </summary>
        internal bool IsBasePatched(Base.Piece basePiece)
        {
            foreach (AquariumPieceMapping aquariumPieceMapping in _aquariumPieceMappingList)
            {
                if (aquariumPieceMapping.BasePiece == basePiece)
                {
                    return aquariumPieceMapping.IsBasePatched;
                }
            }
            return false;
        }

        
        /// <summary>
        /// Sets GhostPatched to true for the given AquariumType 
        /// </summary>
        internal void SetGhostPatched(AquariumType aquariumType)
        {
            foreach (AquariumPieceMapping aquariumPieceMapping in _aquariumPieceMappingList)
            {
                if (aquariumPieceMapping.AquariumType == aquariumType)
                {
                    aquariumPieceMapping.SetGhostPatched();
                }
            }
        }

        /// <summary>
        /// Sets Base Patched to true for the given AquariumType
        /// </summary>
        internal void SetBasePatched(AquariumType aquariumType)
        {
            foreach (AquariumPieceMapping aquariumPieceMapping in _aquariumPieceMappingList)
            {
                if (aquariumPieceMapping.AquariumType == aquariumType)
                {
                    aquariumPieceMapping.SetBasePatched();
                }
            }
        }
        
    }
}