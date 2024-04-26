วางโครงสร้างแบบ clean architecture โดยจะแบ่งเป็น
domain layer ที่เก็บ model,entity ต่างๆ
infrastructure layer ที่ใช้ในการ connect database
application layer ที่เก็บ logic ต่าง
presentation layer คือส่วนแสดงผล ใน project นี้คือ webapi,webapp
เมื่อ  webapi,webapp มีการเชื่อมต่อ database จะเรียกไปที่ application 
แล้ว return ข้อมูลกับมายัง webapi,webapp และนำสาแสดงผล


