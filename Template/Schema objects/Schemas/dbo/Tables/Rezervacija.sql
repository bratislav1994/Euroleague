CREATE TABLE Rezervacija 
    (
     Klub_ID_KLB VARCHAR (64) NOT NULL , 
     Hala_OZN_HALA VARCHAR (64) NOT NULL , 
     PVRMREZ_HALA DATE NOT NULL , 
     KVRMREZ_HALA DATE NOT NULL , 
     SFR_REZ VARCHAR (64) NOT NULL 
    )
    ON "default"
GO