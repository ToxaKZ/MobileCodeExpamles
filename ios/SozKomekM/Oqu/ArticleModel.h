//
//  ArticleModel.h
//  Oqu
//
//  Created by Ельнар Шопанов on 28.02.16.
//  Copyright © 2016 Ельнар Шопанов. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface ArticleModel : NSObject {
    int _id;
    NSString *_name;
    NSString *_dicName;
    NSString *_fromCulture;
    NSString *_toCulture;
}

@property (nonatomic, assign) int id;
@property (nonatomic, copy) NSString *name;
@property (nonatomic, copy) NSString *dicName;
@property (nonatomic, copy) NSString *fromCulture;
@property (nonatomic, copy) NSString *toCulture;

- (id)initWithId:(int)uniqueId name:(NSString*)name dicName:(NSString*)dicName fromCulture:(NSString*)fromCulture toCulture:(NSString*)toCulture;

@end