IF EXISTS (SELECT name FROM sys.indexes  
            WHERE name = N'idx_PolicyInteraction_PolicyId_InteractionTypeId_CreatedBy')   
    DROP INDEX idx_PolicyInteraction_PolicyId_InteractionTypeId_CreatedBy ON PolicyInteraction;   

CREATE NONCLUSTERED INDEX idx_PolicyInteraction_PolicyId_InteractionTypeId_CreatedBy ON [PolicyInteraction] ( [PolicyId], [InteractionTypeId], [CreatedBy] )