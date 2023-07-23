**v2.0.0已经发布，适配全新APP**
https://github.com/LFTEC/LearningFucker/releases/tag/v2.0.0

# LearningFucker
万华学习平台挂学分

> 本库不再公开源码，请直接下载编译后的程序使用。

# 注意
从2022-6-2日之后，UI界面的挂学分功能关闭，请使用CLI模式。

# Cli模式使用方法
发布的Release版本都是不依赖.Net6.0的独立发行版，可以直接下载对应平台的程序执行。

## 激活
为防止滥用，及意外的复制与传递，软件引入了激活机制，仅授权的PC可以使用该软件。

### 申请授权

下载软件后，使用powershell或者cmd，执行如下命令申请授权码。为确保关注本仓库的用户优先体验，建议输入你的github账号。
``` powershell
  .\lf license
```
软件会返回授权申请码，请复制该授权码并在本仓库的Discussions中上传。收到回复后可进行后面的激活操作。  

> 在ps和cmd中，复制是点击鼠标右键，不能使用Ctrl+C


![image](https://github.com/LFTEC/LearningFucker/assets/39933692/ac5060ec-cf12-477a-a85e-dfbc8107f4ce)


### 激活
在收到激活码后，在ps或cmd中运行如下代码激活
``` powershell
  .\lf activate -c 你收到的激活码
```
如果程序返回激活成功，并显示有效期，则在有效期内可以使用该软件。超过有效期后请再次申请授权。


## 启动方法
下载对应平台的压缩包，解压缩。

### Windows
使用Windows Terminal，或者直接运行cmd或powershell，将解压缩的目录指定为工作目录。

#### powershell
```powershell
  .\lf <command> [options]
```

#### cmd
```cmd
  lf <command> [options]
```

命令的简单使用方法可参考下面的示例。详细使用方法请执行 lf --help命令查询。

## 命令使用方法
### 添加和删除用户
```bash
  # 添加用户
  lf adduser -u <用户账户> -p <密码>
```

```bash
  # 删除用户
  lf remove -u <用户账户>
```

### 查询

```bash
  # 查询用户基本信息
  lf list --users
```

```bash
  # 查询本周任务完成情况
  lf list --tasks
```

```bash
  # 查询本周必修课清单
  lf list --courses
```

### 必修课程学习
应对每周要求的必修课程学习，可以指定具体的课程进行学习。如果有考试，在学习完成后自动进行考试。自动考试最多会考允许考试次数的80%，超出将不再进行考试。如果自动考试未通过请自己完成考试。

```bash
  # 必修课程学习
  lf learn --course <课程编号>
```
> 课程编号可通过查询必修课程清单命令获取

### 自动挂学分
程序会自动判断每个用户当前未完成的任务项，按顺序依次进行学习，考试，答题，PK等。

# 更新日志
## Date: 2023-7-12
v2.0.1 Preview
适配更新后的万华学习，提供必修和选修课程的学习和考试功能。

## Date：2022-7-2
发布最新1.1版，使用全新的跨平台.Net 6.0；使用CLI命令行进行学习，不再维护窗口程序。

## Date：2022-6-2
增加每道题答题时间间隔1s，以规避出现答题速度过快的情况。
增加自动答题至允许上限的80%时停止功能，可以在自动答题未完成时，由用户人工答题。

```bash
  # 自动学习
  lf study
```
