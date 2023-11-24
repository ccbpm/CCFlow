
开发者必读： 
==========================

1. 正确的理解 \CCMobilePortal 与 \CCMobile 的两个目录的关系，是如何做为二次开发的基础。 

2. 两个目录平行的放在同一级目录下. 在 \CCMobilePortal 是用户可以个性化的目录，而 CCMobile 则是公共的目录，是ccbpm的核心代码.

3. 核心代码非厂家开发者，或者没有没有经过驰骋公司授权，不得修改，否则不予支持.

4. 在 \CCMobilePortal 下面有两个页面 Home.htm 是登录后的主页，用户可以根据自己的风格进行修改. Login.htm是登录页面，同样二次开发者可以修改.

5. 在 \CCMobile 下，主要有 发起：Start.htm 待办: Todolist.htm , 在途: Runing.htm， 会签:HuiQianList.htm 等通用的页面. 

6. 在 \CCMobile 下的通用页面如果返回主页，都返回到 \CCMobilePortal\Home.htm,  退出返回到  \CCMobilePortal\Login.htm . 

7. 该目录的文件jflow, ccflow通用.
