ALTER TABLE Hala ADD CONSTRAINT Hala_PK PRIMARY KEY CLUSTERED (OZN_HALA)
     WITH (
     ALLOW_PAGE_LOCKS = ON , 
     ALLOW_ROW_LOCKS = ON )
     ON "default" 
    GO