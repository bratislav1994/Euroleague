ALTER TABLE Sudi ADD CONSTRAINT Sudi_PK PRIMARY KEY CLUSTERED (Utakmica_OZN_UTK, Sudija_LICBR_SUD, ID_SUDI)
     WITH (
     ALLOW_PAGE_LOCKS = ON , 
     ALLOW_ROW_LOCKS = ON )
     ON "default" 
    GO