UPDATE RISK
SET SmokerStatusId = CASE	WHEN SmokerStatusId IN (1,2) THEN 6 
							WHEN SmokerStatusId IN (3,4) THEN 5
							ELSE 0 
					 END